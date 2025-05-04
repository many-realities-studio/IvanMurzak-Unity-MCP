#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System.Threading;
using System.Threading.Tasks;
using com.IvanMurzak.Unity.MCP.Common.Data;

namespace com.IvanMurzak.Unity.MCP.Common
{
    public interface IRemoteServer
    {
        Task<ResponseData<string>> NotifyAboutUpdatedTools(CancellationToken cancellationToken = default);
        Task<ResponseData<string>> NotifyAboutUpdatedResources(CancellationToken cancellationToken = default);
    }
}