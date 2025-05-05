#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using com.IvanMurzak.Unity.MCP.Common;
using com.IvanMurzak.Unity.MCP.Common.Data.Unity;
using com.IvanMurzak.Unity.MCP.Utils;

namespace com.IvanMurzak.Unity.MCP.Editor.API
{
    public partial class Tool_GameObject
    {
        [McpPluginTool
        (
            "GameObject_DestroyComponents",
            Title = "Destroy Components from a GameObject in opened Prefab or in a Scene"
        )]
        [Description("Destroy one or many components from target GameObject.")]
        public string DestroyComponents
        (
            [Description("The 'instanceID' array of the target components.")]
            int[] componentInstanceIDs,
            GameObjectRef gameObjectRef
        )
        => MainThread.Run(() =>
        {
            var go = GameObjectUtils.FindBy(gameObjectRef, out var error);
            if (error != null)
                return error;

            var destroyCounter = 0;
            var stringBuilder = new StringBuilder();

            var allComponents = go.GetComponents<UnityEngine.Component>();
            foreach (var component in allComponents)
            {
                var componentFullName = component.GetType().FullName;
                var componentInstanceID = component.GetInstanceID();
                if (componentInstanceIDs.Contains(componentInstanceID))
                {
                    UnityEngine.Object.DestroyImmediate(component);
                    destroyCounter++;
                    stringBuilder.AppendLine($"[Success] Destroyed component instanceID='{componentInstanceID}', type='{componentFullName}'.");
                }
            }

            if (destroyCounter == 0)
                return Error.NotFoundComponents(componentInstanceIDs, allComponents);

            return $"[Success] Destroyed {destroyCounter} components from GameObject.\n{stringBuilder.ToString()}";
        });
    }
}