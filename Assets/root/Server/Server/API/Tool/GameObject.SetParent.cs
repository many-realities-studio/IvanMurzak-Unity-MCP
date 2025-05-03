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
            Name = "GameObject_SetParent",
            Title = "Set parent GameObject in opened Prefab or in a Scene"
        )]
        [Description(@"Set GameObjects in opened Prefab or in a Scene by 'instanceID' (int) array.")]
        public Task<CallToolResponse> SetParent
        (
            [Description("The 'instanceID' array of the target GameObjects.")]
            int[] targetInstanceIDs,
            [Description("The 'instanceID' of the parent GameObject.")]
            int parentInstanceID,
            [Description("A boolean flag indicating whether the GameObject's world position should remain unchanged when setting its parent.")]
            bool worldPositionStays = true
        )
        {
            return ToolRouter.Call("GameObject_SetParent", arguments =>
            {
                arguments[nameof(targetInstanceIDs)] = targetInstanceIDs ?? new int[0];
                arguments[nameof(parentInstanceID)] = parentInstanceID;
                arguments[nameof(worldPositionStays)] = worldPositionStays;
            });
        }
    }
}