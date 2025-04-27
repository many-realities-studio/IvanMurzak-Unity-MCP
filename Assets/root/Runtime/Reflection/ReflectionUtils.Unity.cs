using System.Text;
using com.IvanMurzak.Unity.MCP.Common.Data.Unity;
using com.IvanMurzak.Unity.MCP.Common.Utils;
using UnityEngine;

namespace com.IvanMurzak.Unity.MCP.Utils
{
    public static partial class ReflectionUtils
    {
        public static class Unity
        {
            public static StringBuilder ModifyComponent(Component component, ComponentData data, StringBuilder stringBuilder = null)
            {
                stringBuilder ??= new StringBuilder();

                if (string.IsNullOrEmpty(data.type))
                    return stringBuilder.AppendLine(Error.DataTypeIsEmpty());

                var type = TypeUtils.GetType(data.type);
                if (type == null)
                    return stringBuilder.AppendLine(Error.NotFoundType(data.type));

                var castedComponent = TypeUtils.CastTo(component, data.type, out var error);
                if (error != null)
                    return stringBuilder.AppendLine(error);

                if (!type.IsAssignableFrom(component.GetType()))
                    return stringBuilder.AppendLine(Error.TypeMismatch(data.type, component.GetType().FullName));

                // Enable/Disable component if needed
                if (castedComponent is UnityEngine.Behaviour behaviour)
                {
                    var setEnabled = data.isEnabled.ToBool();
                    if (behaviour.enabled != setEnabled)
                        behaviour.enabled = setEnabled;
                }
                if (castedComponent is UnityEngine.Renderer renderer)
                {
                    var setEnabled = data.isEnabled.ToBool();
                    if (renderer.enabled != setEnabled)
                        renderer.enabled = setEnabled;
                }

                foreach (var field in data.fields ?? new())
                    ModifyField(ref castedComponent, field, stringBuilder);

                foreach (var property in data.properties ?? new())
                    ModifyProperty(ref castedComponent, property, stringBuilder);

                return stringBuilder;
            }
        }
    }
}