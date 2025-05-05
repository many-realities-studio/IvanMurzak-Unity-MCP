#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System.Threading;
using System.Threading.Tasks;
using com.IvanMurzak.Unity.MCP.Common.Data;
using Microsoft.AspNetCore.SignalR.Client;
using R3;

namespace com.IvanMurzak.Unity.MCP.Common
{
    public interface IRpcRouter : IDisposableAsync
    {
        ReadOnlyReactiveProperty<bool> KeepConnected { get; }
        ReadOnlyReactiveProperty<HubConnectionState> ConnectionState { get; }
        Task<bool> Connect(CancellationToken cancellationToken = default);
        Task Disconnect(CancellationToken cancellationToken = default);

        Task<ResponseData<string>> NotifyAboutUpdatedTools(CancellationToken cancellationToken = default);
        Task<ResponseData<string>> NotifyAboutUpdatedResources(CancellationToken cancellationToken = default);
    }
}