#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using R3;

namespace com.IvanMurzak.Unity.MCP.Common
{
    public partial class McpPlugin : IMcpPlugin
    {
        public const string Version = "0.7.0";

        readonly ILogger<McpPlugin> _logger;
        readonly IRpcRouter _rpcRouter;

        public IMcpRunner McpRunner { get; private set; }
        public IRemoteServer RemoteServer { get; private set; }
        public ReadOnlyReactiveProperty<HubConnectionState> ConnectionState => _rpcRouter.ConnectionState;
        public ReadOnlyReactiveProperty<bool> KeepConnected => _rpcRouter.KeepConnected;

        public McpPlugin(ILogger<McpPlugin> logger, IRpcRouter rpcRouter, IMcpRunner mcpRunner, IRemoteServer remoteServer)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _logger.LogTrace("{0} Ctor. Version: {Version}", typeof(McpPlugin).Name, Version);

            _rpcRouter = rpcRouter ?? throw new ArgumentNullException(nameof(rpcRouter));

            McpRunner = mcpRunner ?? throw new ArgumentNullException(nameof(mcpRunner));
            RemoteServer = remoteServer ?? throw new ArgumentNullException(nameof(remoteServer));

            if (HasInstance)
            {
                _logger.LogError("Connector already created. Use Singleton instance.");
                return;
            }

            _instance.Value = this;
        }

        public Task<bool> Connect(CancellationToken cancellationToken = default)
        {
            RemoteServer?.Connect(cancellationToken);
            return _rpcRouter.Connect(cancellationToken).ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully && task.Result)
                    RemoteServer?.NotifyAboutUpdatedTools();
                return task.IsCompletedSuccessfully;
            });
        }

        public Task Disconnect(CancellationToken cancellationToken = default)
        {
            RemoteServer?.Disconnect(cancellationToken);
            return _rpcRouter.Disconnect(cancellationToken);
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
            var localInstance = _instance.CurrentValue;
            if (localInstance == this)
                _instance.Value = null;

            try
            {
                await _rpcRouter.DisposeAsync();
                await RemoteServer.DisposeAsync();

                if (localInstance != null)
                    await localInstance.DisposeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error during async disposal: {0}\n{1}", ex.Message, ex.StackTrace);
            }
        }

        ~McpPlugin() => Dispose();
    }
}