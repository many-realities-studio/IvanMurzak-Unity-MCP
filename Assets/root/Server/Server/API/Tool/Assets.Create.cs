using ModelContextProtocol.Protocol.Types;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Threading.Tasks;

namespace com.IvanMurzak.Unity.MCP.Server.API
{
    public partial class Tool_Assets
    {
        [McpServerTool
        (
            Name = "Assets_Create",
            Title = "Assets Create"
        )]
        [Description(@"Create new asset.")]
        public Task<CallToolResponse> Create()
        {
            return ToolRouter.Call("Assets_Create");
        }
    }
}