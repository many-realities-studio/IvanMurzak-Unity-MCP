#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System.ComponentModel;
using System.Text;
using com.IvanMurzak.Unity.MCP.Common;
using com.IvanMurzak.Unity.MCP.Common.Data.Unity;
using com.IvanMurzak.Unity.MCP.Common.Utils;
using com.IvanMurzak.Unity.MCP.Utils;

namespace com.IvanMurzak.Unity.MCP.Editor.API
{
    public partial class Tool_GameObject
    {
        [McpPluginTool
        (
            "GameObject_AddComponent",
            Title = "Add Component to a GameObject in opened Prefab or in a Scene"
        )]
        [Description("Add a component to a GameObject.")]
        public string AddComponent
        (
            [Description("Full name of the Component. It should include full namespace path and the class name.")]
            string[] componentNames,
            GameObjectRef gameObjectRef
        )
        => MainThread.Run(() =>
        {
            var go = GameObjectUtils.FindBy(gameObjectRef, out var error);
            if (error != null)
                return error;

            if ((componentNames?.Length ?? 0) == 0)
                return $"[Error] No component names provided.";

            var stringBuilder = new StringBuilder();
            
            foreach (var componentName in componentNames)
            {
                var type = TypeUtils.GetType(componentName);
                if (type == null)
                {
                    stringBuilder.AppendLine(Tool_Component.Error.NotFoundComponentType(componentName));
                    continue;
                }

                // Check if type is a subclass of UnityEngine.Component
                if (!typeof(UnityEngine.Component).IsAssignableFrom(type))
                {
                    stringBuilder.AppendLine(Tool_Component.Error.TypeMustBeComponent(componentName));
                    continue;
                }

                var newComponent = go.AddComponent(type);

                stringBuilder.AppendLine($"[Success] Added component '{componentName}'. Component instanceID='{newComponent.GetInstanceID()}'.");
            }

            stringBuilder.AppendLine(go.Print());
            return stringBuilder.ToString();
        });
    }
}