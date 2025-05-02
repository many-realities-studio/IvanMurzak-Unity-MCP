#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System.ComponentModel;
using System.Text;
using com.IvanMurzak.Unity.MCP.Common;
using com.IvanMurzak.Unity.MCP.Utils;
using UnityEditor.SceneManagement;

namespace com.IvanMurzak.Unity.MCP.Editor.API
{
    public partial class Tool_GameObject
    {
        [McpPluginTool
        (
            "GameObject_SetParent",
            Title = "Set parent GameObject in opened Prefab or in a Scene"
        )]
        [Description(@"Set GameObjects in opened Prefab or in a Scene by 'instanceID' (int) array.")]
        public string SetParent
        (
            [Description("The 'instanceID' array of the target GameObjects.")]
            int[] targetInstanceIDs,
            [Description("The 'instanceID' of the parent GameObject.")]
            int parentInstanceID,
            [Description("A boolean flag indicating whether the GameObject's world position should remain unchanged when setting its parent.")]
            bool worldPositionStays = true
        )
        {
            return MainThread.Run(() =>
            {
                var stringBuilder = new StringBuilder();
                int changedCount = 0;

                for (var i = 0; i < targetInstanceIDs.Length; i++)
                {
                    var targetInstanceID = targetInstanceIDs[i];

                    var targetGo = GameObjectUtils.FindByInstanceID(targetInstanceID);
                    if (targetGo == null)
                    {
                        stringBuilder.AppendLine($"[Error] Target GameObject with instanceID {targetInstanceID} not found.");
                        continue;
                    }

                    var parentGo = GameObjectUtils.FindByInstanceID(parentInstanceID);
                    if (parentGo == null)
                    {
                        stringBuilder.AppendLine($"[Error] Parent GameObject with instanceID {parentInstanceID} not found.");
                        continue;
                    }

                    targetGo.transform.SetParent(parentGo.transform, worldPositionStays: worldPositionStays);
                    changedCount++;

                    stringBuilder.AppendLine(@$"[Success] Set parent of GameObject with instanceID {targetInstanceID} to GameObject with instanceID {parentInstanceID}.");
                }

                if (changedCount > 0)
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

                return stringBuilder.ToString();
            });
        }
    }
}