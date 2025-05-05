#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System.Text.Json;
using com.IvanMurzak.Unity.MCP.Common.Data.Utils;

namespace com.IvanMurzak.Unity.MCP.Utils
{
    public static class ExtensionsObjectRef
    {
        public static UnityEngine.Object? FindObject(this ObjectRef objectRef)
        {
            if (objectRef == null)
                return null;

#if UNITY_EDITOR
            if (objectRef.instanceID != 0)
                return UnityEditor.EditorUtility.InstanceIDToObject(objectRef.instanceID);

            if (!string.IsNullOrEmpty(objectRef.assetPath))
                return UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(objectRef.assetPath);
#endif

            return null;
        }

        public static ObjectRef? ToObjectRef(this JsonElement? jsonElement)
        {
            if (jsonElement == null)
                return null;

            try
            {
                return jsonElement.HasValue
                    ? JsonSerializer.Deserialize<ObjectRef>(jsonElement.Value)
                    : null;
            }
            // catch (JsonException ex)
            // {
            //     if (McpPluginUnity.LogLevel.IsActive(LogLevel.Error))
            //         UnityEngine.Debug.LogError($"Failed to deserialize ObjectRef: {ex.Message}");
            //     return null;
            // }
            catch
            {
                return null;
            }
        }
    }
}