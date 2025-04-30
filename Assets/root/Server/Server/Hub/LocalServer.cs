#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System.Threading.Tasks;
using com.IvanMurzak.Unity.MCP.Common.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using R3;

namespace com.IvanMurzak.Unity.MCP.Server
{
    public class LocalServer : BaseHub<LocalServer>, ILocalServer
    {
        static readonly Subject<Unit> _onListToolUpdated = new();
        public Observable<Unit> OnListToolUpdated => _onListToolUpdated;

        static readonly Subject<Unit> _onListResourcesUpdated = new();
        public Observable<Unit> OnListResourcesUpdated => _onListResourcesUpdated;


        public LocalServer(ILogger<LocalServer> logger, IHubContext<LocalServer> hubContext) : base(logger, hubContext)
        {
        }

        public Task<IResponseData<string>> SetOnListToolsUpdated(string data)
        {
            _logger.LogTrace("LocalServer SetOnListToolsUpdated. {0}. Data: {1}", _guid, data);
            _onListToolUpdated.OnNext(Unit.Default);
            return ResponseData<string>.Success(data, string.Empty).TaskFromResult();
        }

        public Task<IResponseData<string>> SetOnListResourcesUpdated(string data)
        {
            _logger.LogTrace("LocalServer SetOnListResourcesUpdated. {0}. Data: {1}", _guid, data);
            _onListResourcesUpdated.OnNext(Unit.Default);
            return ResponseData<string>.Success(data, string.Empty).TaskFromResult();
        }
    }
}