#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.IvanMurzak.Unity.MCP.Common;
using com.IvanMurzak.Unity.MCP.Common.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace com.IvanMurzak.Unity.MCP.Server
{
    public class RemoteApp : BaseHub<RemoteApp>, IRemoteApp
    {
        public static IEnumerable<string> AllConnections => ConnectedClients.TryGetValue(typeof(RemoteApp), out var clients)
            ? clients?.Keys ?? new string[0]
            : Enumerable.Empty<string>();

        public static string? FirstConnectionId => ConnectedClients.TryGetValue(typeof(RemoteApp), out var clients)
            ? clients?.FirstOrDefault().Key
            : null;
        
        readonly EventAppToolsChange _eventAppToolsChange;

        public RemoteApp(ILogger<RemoteApp> logger, IHubContext<RemoteApp> hubContext, EventAppToolsChange eventAppToolsChange)
            : base(logger, hubContext)
        {
            _eventAppToolsChange = eventAppToolsChange ?? throw new ArgumentNullException(nameof(eventAppToolsChange));
        }

        public Task<IResponseData<string>> OnListToolsUpdated(string data)
        {
            _logger.LogTrace("RemoteApp OnListToolsUpdated. {0}. Data: {1}", _guid, data);
            _eventAppToolsChange.OnNext(new EventAppToolsChange.EventData
            {
                ConnectionId = Context.ConnectionId,
                Data = data
            });
            return ResponseData<string>.Success(data, string.Empty).TaskFromResult<IResponseData<string>>();
        }

        public Task<IResponseData<string>> OnListResourcesUpdated(string data)
        {
            _logger.LogTrace("RemoteApp OnListResourcesUpdated. {0}. Data: {1}", _guid, data);
            // _onListResourcesUpdated.OnNext(Unit.Default);
            return ResponseData<string>.Success(data, string.Empty).TaskFromResult<IResponseData<string>>();
        }
    }
}