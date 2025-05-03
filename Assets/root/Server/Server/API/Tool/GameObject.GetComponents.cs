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
            [Description("GameObject by 'instanceID' (int). Priority: 1. (Recommended)")]
            int instanceID = 0,
            [Description("GameObject by 'path'. Priority: 2.")]
            string? path = null,
            [Description("GameObject by 'name'. Priority: 3.")]
            string? name = null
        )
        {
            return ToolRouter.Call("GameObject_GetComponents", arguments =>
            {
                arguments[nameof(componentInstanceIDs)] = componentInstanceIDs;
                arguments[nameof(instanceID)] = instanceID;

                if (path != null && path.Length > 0)
                    arguments[nameof(path)] = path;

                if (name != null && name.Length > 0)
                    arguments[nameof(name)] = name;
            });
        }
    }
}