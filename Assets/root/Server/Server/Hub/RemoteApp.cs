#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using System.Threading.Tasks;
using com.IvanMurzak.Unity.MCP.Common;
using com.IvanMurzak.Unity.MCP.Common.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace com.IvanMurzak.Unity.MCP.Server
{
    public class RemoteApp : BaseHub<RemoteApp>, IRemoteApp
    {
        readonly EventAppToolsChange _eventAppToolsChange;

        public RemoteApp(ILogger<RemoteApp> logger, IHubContext<RemoteApp> hubContext, EventAppToolsChange eventAppToolsChange)
            : base(logger, hubContext)
        {
            _eventAppToolsChange = eventAppToolsChange ?? throw new ArgumentNullException(nameof(eventAppToolsChange));
        }

        public Task<IResponseData<string>> SetOnListToolsUpdated(string data)
        {
            _logger.LogTrace("LocalServer SetOnListToolsUpdated. {0}. Data: {1}", _guid, data);
            _eventAppToolsChange.OnNext(new EventAppToolsChange.EventData
            {
                ConnectionId = Context.ConnectionId,
                Data = data
            });
            return ResponseData<string>.Success(data, string.Empty).TaskFromResult<IResponseData<string>>();
        }

        public Task<IResponseData<string>> SetOnListResourcesUpdated(string data)
        {
            _logger.LogTrace("LocalServer SetOnListResourcesUpdated. {0}. Data: {1}", _guid, data);
            // _onListResourcesUpdated.OnNext(Unit.Default);
            return ResponseData<string>.Success(data, string.Empty).TaskFromResult<IResponseData<string>>();
        }

        // public async Task<IResponseData<ResponseListTool[]>> RunListTool(IRequestListTool data, CancellationToken cancellationToken = default)
        // {
        //     try
        //     {
        //         var client = GetActiveClient();
        //         if (client == null)
        //             return ResponseData<ResponseListTool[]>.Error(data.RequestID, $"No connected clients for {GetType().Name}.")
        //                 .Log(_logger);

        //         var result = await client.InvokeAsync<ResponseData<ResponseListTool[]>>(Consts.RPC.Client.RunListTool, data, cancellationToken);
        //         if (result == null)
        //             return ResponseData<ResponseListTool[]>.Error(data.RequestID, $"'{Consts.RPC.Client.RunListTool}' returned null result.")
        //                 .Log(_logger);

        //         return result;
        //     }
        //     catch (Exception ex)
        //     {
        //         return ResponseData<ResponseListTool[]>.Error(data.RequestID, $"Failed to run '{Consts.RPC.Client.RunListTool}'. Exception: {ex}")
        //             .Log(_logger, ex);
        //     }
        // }

        // public async Task<IResponseData<ResponseResourceContent[]>> RunResourceContent(IRequestResourceContent data, CancellationToken cancellationToken = default)
        // {
        //     if (data == null)
        //         return ResponseData<ResponseResourceContent[]>.Error(Consts.Guid.Zero, "Resource content data is null.")
        //             .Log(_logger);

        //     if (string.IsNullOrEmpty(data.Uri))
        //         return ResponseData<ResponseResourceContent[]>.Error(data.RequestID, "Resource content Uri is null.")
        //             .Log(_logger);

        //     try
        //     {
        //         var client = GetActiveClient();
        //         if (client == null)
        //             return ResponseData<ResponseResourceContent[]>.Error(data.RequestID, $"No connected clients for {GetType().Name}.")
        //                 .Log(_logger);

        //         var result = await client.InvokeAsync<ResponseData<ResponseResourceContent[]>>(Consts.RPC.Client.RunResourceContent, data, cancellationToken);
        //         if (result == null)
        //             return ResponseData<ResponseResourceContent[]>.Error(data.RequestID, $"Resource uri: '{data.Uri}' returned null result.")
        //                 .Log(_logger);

        //         return result;
        //     }
        //     catch (Exception ex)
        //     {
        //         return ResponseData<ResponseResourceContent[]>.Error(data.RequestID, $"Failed to get resource uri: '{data.Uri}'. Exception: {ex}")
        //             .Log(_logger, ex);
        //     }
        // }

        // public async Task<IResponseData<ResponseListResource[]>> RunListResources(IRequestListResources data, CancellationToken cancellationToken = default)
        // {
        //     try
        //     {
        //         var client = GetActiveClient();
        //         if (client == null)
        //             return ResponseData<ResponseListResource[]>.Error(data.RequestID, $"No connected clients for {GetType().Name}.")
        //                 .Log(_logger);

        //         var result = await client.InvokeAsync<ResponseData<ResponseListResource[]>>(Consts.RPC.Client.RunListResources, data, cancellationToken);
        //         if (result == null)
        //             return ResponseData<ResponseListResource[]>.Error(data.RequestID, $"'{Consts.RPC.Client.RunListResources}' returned null result.")
        //                 .Log(_logger);

        //         return result;
        //     }
        //     catch (Exception ex)
        //     {
        //         return ResponseData<ResponseListResource[]>.Error(data.RequestID, $"Failed to run '{Consts.RPC.Client.RunListResources}'. Exception: {ex}")
        //             .Log(_logger, ex);
        //     }
        // }

        // public async Task<IResponseData<ResponseResourceTemplate[]>> RunResourceTemplates(IRequestListResourceTemplates data, CancellationToken cancellationToken = default)
        // {
        //     try
        //     {
        //         var client = GetActiveClient();
        //         if (client == null)
        //             return ResponseData<ResponseResourceTemplate[]>.Error(data.RequestID, $"No connected clients for {GetType().Name}.")
        //                 .Log(_logger);

        //         var result = await client.InvokeAsync<ResponseData<ResponseResourceTemplate[]>>(Consts.RPC.Client.RunListResourceTemplates, data, cancellationToken);
        //         if (result == null)
        //             return ResponseData<ResponseResourceTemplate[]>.Error(data.RequestID, $"'{Consts.RPC.Client.RunListResourceTemplates}' returned null result.")
        //                 .Log(_logger);

        //         return result;
        //     }
        //     catch (Exception ex)
        //     {
        //         return ResponseData<ResponseResourceTemplate[]>.Error(data.RequestID, $"Failed to run '{Consts.RPC.Client.RunListResourceTemplates}'. Exception: {ex}")
        //             .Log(_logger, ex);
        //     }
        // }

        public new void Dispose()
        {
            base.Dispose();
        }
    }
}