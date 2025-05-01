#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using R3;

namespace com.IvanMurzak.Unity.MCP.Common
{
    public class ConnectionManager : IConnectionManager
    {
        public const string Version = "0.1.0";

        readonly ILogger<ConnectionManager> _logger;
        readonly ReactiveProperty<HubConnection> _hubConnection = new();
        readonly Func<string, Task<HubConnection>> _hubConnectionBuilder;
        readonly ReactiveProperty<HubConnectionState> _connectionState = new(HubConnectionState.Disconnected);
        readonly ReactiveProperty<bool> _continueToReconnect = new(false);

        Task<bool>? connectionTask;
        HubConnectionLogger? hubConnectionLogger;
        HubConnectionObservable? hubConnectionObservable;
        CancellationTokenSource? internalCts;
        public ReadOnlyReactiveProperty<HubConnectionState> ConnectionState => _connectionState.ToReadOnlyReactiveProperty();
        public ReadOnlyReactiveProperty<HubConnection> HubConnection => _hubConnection.ToReadOnlyReactiveProperty();
        public ReadOnlyReactiveProperty<bool> KeepConnected => _continueToReconnect.ToReadOnlyReactiveProperty();
        public string Endpoint { get; set; } = string.Empty;

        public ConnectionManager(ILogger<ConnectionManager> logger, Func<string, Task<HubConnection>> hubConnectionBuilder)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _logger.LogTrace("Ctor. Version: {0}", Version);

            _hubConnectionBuilder = hubConnectionBuilder ?? throw new ArgumentNullException(nameof(hubConnectionBuilder));
            _hubConnection.Subscribe(hubConnection =>
            {
                if (hubConnection == null)
                {
                    _connectionState.Value = HubConnectionState.Disconnected;
                    return;
                }

                hubConnection.ToObservable().State
                    .Subscribe(state => _connectionState.Value = state);
            });
        }

        public async Task InvokeAsync<TInput>(string methodName, TInput input, CancellationToken cancellationToken = default)
        {
            if (_hubConnection.Value?.State != HubConnectionState.Connected)
            {
                await Connect(cancellationToken);
                if (_hubConnection.Value?.State != HubConnectionState.Connected)
                {
                    _logger.LogError("Can't establish connection with Remote.");
                    return;
                }
            }

            await _hubConnection.Value.InvokeAsync(methodName, input, cancellationToken).ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                    return;

                _logger.LogError("Failed to invoke method {0}: {1}", methodName, task.Exception?.Message);
            });
        }

        public async Task<TResult> InvokeAsync<TInput, TResult>(string methodName, TInput input, CancellationToken cancellationToken = default)
        {
            if (_hubConnection.Value?.State != HubConnectionState.Connected)
            {
                await Connect(cancellationToken);
                if (_hubConnection.Value?.State != HubConnectionState.Connected)
                {
                    _logger.LogError("Can't establish connection with Remote.");
                    return default!;
                }
            }

            return await _hubConnection.Value.InvokeAsync<TResult>(methodName, input, cancellationToken).ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                    return task.Result;

                _logger.LogError("Failed to invoke method {0}: {1}", methodName, task.Exception?.Message);
                return default!;
            });
        }

        public async Task<bool> Connect(CancellationToken cancellationToken = default)
        {
            _continueToReconnect.Value = false;

            // Dispose the previous internal CancellationTokenSource if it exists
            CancelInternalToken(dispose: true);

            if (_hubConnection.Value != null)
                await _hubConnection.Value.StopAsync();

            _continueToReconnect.Value = true;

            if (_hubConnection.Value?.State == HubConnectionState.Connected)
                return true;

            internalCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            if (connectionTask != null)
            {
                _logger.LogDebug("Connection task already exists. Waiting for the completion... {0}.", Endpoint);
                // Create a new task that waits for the existing task but can be canceled independently
                return await Task.Run(async () =>
                {
                    try
                    {
                        await connectionTask; // Wait for the existing connection task
                        return _hubConnection.Value?.State == HubConnectionState.Connected;
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogWarning("Connection task was canceled {0}.", Endpoint);
                        return false;
                    }
                }, internalCts.Token);
            }

            try
            {
                connectionTask = InternalConnect(internalCts.Token);
                return await connectionTask;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error during connection: {0}\n{1}", ex.Message, ex.StackTrace);
                return false;
            }
            finally
            {
                connectionTask = null;
            }
        }

        void CancelInternalToken(bool dispose = false)
        {
            if (internalCts != null)
            {
                if (!internalCts.IsCancellationRequested)
                    internalCts.Cancel();

                if (dispose)
                {
                    internalCts.Dispose();
                    internalCts = null;
                }
            }
        }

        async Task<bool> InternalConnect(CancellationToken cancellationToken)
        {
            if (_hubConnection.Value == null)
            {
                _logger.LogDebug("Creating new HubConnection instance {0}.", Endpoint);
                var hubConnection = await _hubConnectionBuilder(Endpoint);
                if (hubConnection == null)
                {
                    _logger.LogError("Can't create connection instance. Something may be wrong with Connection Config {0}.", Endpoint);
                    return false;
                }

                _hubConnection.Value = hubConnection;

                hubConnectionLogger?.Dispose();
                hubConnectionLogger = new(_logger, hubConnection);

                hubConnectionObservable?.Dispose();
                hubConnectionObservable = new(hubConnection);
                hubConnectionObservable.Closed
                    .Where(_ => _continueToReconnect.CurrentValue)
                    .Subscribe(async _ =>
                    {
                        connectionTask = null;
                        if (cancellationToken.IsCancellationRequested)
                            return;
                        _logger.LogWarning("Connection closed. Attempting to reconnect... {0}.", Endpoint);
                        await InternalConnect(cancellationToken);
                    })
                    .RegisterTo(cancellationToken);
            }

            _logger.LogDebug("Connecting to {0}...", Endpoint);
            while (_continueToReconnect.CurrentValue && !cancellationToken.IsCancellationRequested)
            {
                var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                var task = _hubConnection.CurrentValue.StartAsync(cts.Token);
                try
                {
                    await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(3), cancellationToken));
                    if (!task.IsCompletedSuccessfully)
                    {
                        if (_continueToReconnect.CurrentValue && !cancellationToken.IsCancellationRequested)
                        {
                            _logger.LogTrace("Waiting before retry... {0}", Endpoint);
                            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken); // Wait before retrying
                        }
                        continue;
                    }
                    _logger.LogInformation("Connection started successfully {0}.", Endpoint);
                    _connectionState.Value = HubConnectionState.Connected;
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Failed to start connection. {0} - {1}\n{2}", Endpoint, ex.Message, ex.StackTrace);
                }
                finally
                {
                    cts.Cancel();
                    cts.Dispose();
                }
                if (_continueToReconnect.CurrentValue && !cancellationToken.IsCancellationRequested)
                {
                    _logger.LogTrace("Waiting before retry... {0}", Endpoint);
                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken); // Wait before retrying
                }
            }
            return false;
        }

        public Task Disconnect(CancellationToken cancellationToken = default)
        {
            connectionTask = null;
            _continueToReconnect.Value = false;

            // Cancel the internal token to stop any ongoing connection attempts
            CancelInternalToken(dispose: false);

            if (_hubConnection.Value == null)
                return Task.CompletedTask;

            return _hubConnection.Value.StopAsync(cancellationToken).ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    _logger.LogInformation("HubConnection stopped successfully.");
                }
                else if (task.Exception != null)
                {
                    _logger.LogError("Error while stopping HubConnection: {0}\n{1}", task.Exception.Message, task.Exception.StackTrace);
                }
                _connectionState.Value = HubConnectionState.Disconnected;
            });
        }

        public void Dispose()
        {
#pragma warning disable CS4014
            DisposeAsync();
            // DisposeAsync().Wait();
            // Unity won't reload Domain if we call DisposeAsync().Wait() here.
#pragma warning restore CS4014
        }

        public async Task DisposeAsync()
        {
            connectionTask = null;
            if (!_continueToReconnect.IsDisposed)
                _continueToReconnect.Value = false;

            hubConnectionLogger?.Dispose();
            hubConnectionObservable?.Dispose();

            _connectionState.Dispose();
            _continueToReconnect.Dispose();

            CancelInternalToken(dispose: true);

            if (_hubConnection.CurrentValue != null)
            {
                try
                {
                    var tempHubConnection = _hubConnection.Value;
                    _hubConnection.Dispose();
                    await tempHubConnection.StopAsync()
                        .ContinueWith(task =>
                        {
                            try
                            {
                                tempHubConnection.DisposeAsync();
                            }
                            catch { }
                        });
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error during async disposal: {0}\n{1}", ex.Message, ex.StackTrace);
                }
            }
            else
            {
                _hubConnection.Dispose();
            }
        }

        ~ConnectionManager() => Dispose();
    }
}