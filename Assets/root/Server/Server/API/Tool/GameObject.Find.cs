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
            Name = "GameObject_Find",
            Title = "Find GameObject in opened Prefab or in a Scene"
        )]
        [Description(@"Finds specific GameObject by provided information.
First it looks for the opened Prefab, if any Prefab is opened it looks only there ignoring a scene.
If no opened Prefab it looks into current active scene.
Returns GameObject infromation and its children.
Also, it returns Components preview just for the target GameObject.")]
        public Task<CallToolResponse> Find
        (
            [Description("Determines the depth of the hierarchy to include. 0 - means only the target GameObject. 1 - means to include one layer below.")]
            int includeChildrenDepth = 0,
            [Description("Find by 'instanceID' (int). Priority: 1. (Recommended)")]
            int instanceID = 0,
            [Description("Find by 'path'. Priority: 2.")]
            string? path = null,
            [Description("Find by 'name'. Priority: 3.")]
            string? name = null
        )
        {
            return ToolRouter.Call("GameObject_Find", arguments =>
            {
                arguments[nameof(includeChildrenDepth)] = includeChildrenDepth;
                arguments[nameof(instanceID)] = instanceID;

                if (path != null && path.Length > 0)
                    arguments[nameof(path)] = path;

                if (name != null && name.Length > 0)
                    arguments[nameof(name)] = name;
            });
        }
    }
}