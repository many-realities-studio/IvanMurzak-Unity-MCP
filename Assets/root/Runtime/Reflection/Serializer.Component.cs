using System.Reflection;
using UnityEngine;
using com.IvanMurzak.Unity.MCP.Common;
using com.IvanMurzak.Unity.MCP.Common.Data.Unity;
namespace com.IvanMurzak.Unity.MCP.Utils
{
    /// <summary>
    /// Serializes Unity components to JSON format.
    /// </summary>
    public static partial class Serializer
    {
        public static class Component
        {
            /// <summary>
            /// Serializes the specified component to a JSON string.
            /// </summary>
            /// <param name="component">The component to serialize.</param>
            /// <returns>A JSON string representing the component's data.</returns>
            public static string Serialize(UnityEngine.Component component)
            {
                if (component == null)
                    return null;

                var list = BuildData(component);
                return JsonUtils.JsonSerialize(list);
            }
            public static string SerializeLight(UnityEngine.Component component)
            {
                if (component == null)
                    return null;

                var list = BuildDataLight(component);
                return JsonUtils.JsonSerialize(list);
            }
            public static ComponentDataLight BuildDataLight(UnityEngine.Component component)
            {
                if (component == null)
                    return null;

                return new ComponentDataLight()
                {
                    type = component.GetType().FullName,
                    isEnabled = BuildIsEnabled(component),
                    instanceID = component.GetInstanceID(),
                };
            }
            public static ComponentData.Enabled BuildIsEnabled(UnityEngine.Component component)
            {
                if (component == null)
                    return ComponentData.Enabled.NA;

                if (component is Behaviour behaviour)
                    return behaviour.enabled
                        ? ComponentData.Enabled.True
                        : ComponentData.Enabled.False;

                if (component is Renderer renderer)
                    return renderer.enabled
                        ? ComponentData.Enabled.True
                        : ComponentData.Enabled.False;

                return ComponentData.Enabled.NA;
            }
            public static ComponentData BuildData(UnityEngine.Component component)
            {
                if (component == null)
                    return null;

                var result = new ComponentData()
                {
                    type = component.GetType().FullName,
                    isEnabled = BuildIsEnabled(component),
                    instanceID = component.GetInstanceID()
                };

                var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

                result.fields = Anything.SerializeFields(component, flags);
                result.properties = Anything.SerializeProperties(component, flags);

                return result;
            }
        }
    }
}