using com.IvanMurzak.Unity.MCP.Common.Data.Unity;
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
            Name = "GameObject_GetComponents",
            Title = "Get GameObject components in opened Prefab or in a Scene"
        )]
        [Description(@"Get components of the target GameObject. Returns property values of each component.
Returns list of all available components preview if no requested components found.")]
        public Task<CallToolResponse> GetComponents
        (
            [Description("The 'instanceID' array of the target components. Leave it empty if all components needed.")]
            int[] componentInstanceIDs,
            GameObjectRef gameObjectRef
        )
        {
            return ToolRouter.Call("GameObject_GetComponents", arguments =>
            {
                arguments[nameof(componentInstanceIDs)] = componentInstanceIDs;
                arguments[nameof(gameObjectRef)] = gameObjectRef;
            });
        }
    }
}