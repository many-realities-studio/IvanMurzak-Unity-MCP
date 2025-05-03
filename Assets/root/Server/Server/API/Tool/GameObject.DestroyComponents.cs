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
            Name = "GameObject_DestroyComponents",
            Title = "Destroy Components from a GameObject in opened Prefab or in a Scene"
        )]
        [Description("Destroy one or many components from target GameObject.")]
        public Task<CallToolResponse> DestroyComponents
        (
            [Description("The 'instanceID' array of the target components.")]
            int[] componentInstanceIDs,
            [Description("GameObject by 'instanceID' (int). Priority: 1. (Recommended)")]
            int instanceID = 0,
            [Description("GameObject by 'path'. Priority: 2.")]
            string? path = null,
            [Description("GameObject by 'name'. Priority: 3.")]
            string? name = null
        )
        {
            return ToolRouter.Call("GameObject_DestroyComponents", arguments =>
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