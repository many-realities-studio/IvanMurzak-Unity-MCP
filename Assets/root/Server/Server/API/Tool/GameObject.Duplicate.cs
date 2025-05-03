using ModelContextProtocol.Protocol.Types;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Threading.Tasks;

namespace com.IvanMurzak.Unity.MCP.Server.API
{
    public partial class Tool_GameObject
    {
        [McpServerTool
        (
            Name = "GameObject_Duplicate",
            Title = "Duplicate GameObjects in opened Prefab and in a Scene"
        )]
        [Description(@"Duplicate GameObjects in opened Prefab and in a Scene by 'instanceID' (int) array.")]
        public Task<CallToolResponse> Duplicate
        (
            [Description("The 'instanceID' array of the target GameObjects.")]
            int [] instanceIDs
        )
        {
            return ToolRouter.Call("GameObject_Duplicate", arguments =>
            {
                arguments[nameof(instanceIDs)] = instanceIDs;
            });
        }
    }
}