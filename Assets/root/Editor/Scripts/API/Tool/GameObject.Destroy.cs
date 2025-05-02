#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System.ComponentModel;
using com.IvanMurzak.Unity.MCP.Common;
using com.IvanMurzak.Unity.MCP.Utils;
using UnityEngine;

namespace com.IvanMurzak.Unity.MCP.Editor.API
{
    public partial class Tool_GameObject
    {
        [McpPluginTool
        (
            "GameObject_Destroy",
            Title = "Destroy GameObject in opened Prefab or in a Scene"
        )]
        [Description(@"Destroy a GameObject and all nested GameObjects recursively.
Use 'instanceID' whenever possible, because it finds the exact GameObject, when 'path' may find a wrong one.")]
        public string Destroy
        (
            [Description("Delete by 'instanceID' (int). Priority: 1. (Recommended)")]
            int instanceID = 0,
            [Description("Delete by 'path'. Priority: 2.")]
            string? path = null,
            [Description("Delete by 'name'. Priority: 3.")]
            string? name = null
        )
        => MainThread.Run(() =>
        {
            // Find by 'instanceID' first, then by 'path', then by 'name'
            var go = GameObjectUtils.FindBy(instanceID, path, name, out var error);
            if (error != null)
                return error;

            Object.DestroyImmediate(go);
            return $"[Success] Destroy GameObject.";
        });
    }
}