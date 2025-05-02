#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System.ComponentModel;
using System.Linq;
using com.IvanMurzak.Unity.MCP.Common;
using com.IvanMurzak.Unity.MCP.Utils;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace com.IvanMurzak.Unity.MCP.Editor.API
{
    public partial class Tool_GameObject
    {
        [McpPluginTool
        (
            "GameObject_Duplicate",
            Title = "Duplicate GameObjects in opened Prefab and in a Scene"
        )]
        [Description(@"Duplicate GameObjects in opened Prefab and in a Scene by 'instanceID' (int) array.")]
        public string Duplicate
        (
            [Description("The 'instanceID' array of the target GameObjects.")]
            int [] instanceIDs
        )
        {
            return MainThread.Run(() =>
            {
                var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

                Selection.instanceIDs = instanceIDs;

                Unsupported.DuplicateGameObjectsUsingPasteboard();

                var modifiedScenes = Selection.gameObjects
                    .Select(go => go.scene)
                    .Distinct()
                    .ToList();

                foreach (var scene in modifiedScenes)
                    EditorSceneManager.MarkSceneDirty(scene);

                var location = prefabStage != null ? "Prefab" : "Scene";
                return @$"[Success] Duplicated {instanceIDs.Length} GameObjects in opened {location} by 'instanceID' (int) array.
Duplicated instanceIDs:
{string.Join(", ", Selection.instanceIDs)}";
            });
        }
    }
}