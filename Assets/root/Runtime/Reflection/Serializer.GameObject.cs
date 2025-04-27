using com.IvanMurzak.Unity.MCP.Common;
using com.IvanMurzak.Unity.MCP.Common.Data.Unity;
using System.Linq;

namespace com.IvanMurzak.Unity.MCP.Utils
{
    /// <summary>
    /// Serializes Unity components to JSON format.
    /// </summary>
    public static partial class Serializer
    {
        public static class GameObject
        {
            public static GameObjectData BuildData(UnityEngine.GameObject go)
            {
                if (go == null)
                    return null;

                return new GameObjectData()
                {
                    name = go.name,
                    tag = go.tag,
                    layer = go.layer,
                    instanceID = go.GetInstanceID(),
                    components = go.GetComponents<UnityEngine.Component>()
                        .Select((c, i) => Serializer.Serialize(c, name: $"[{i}]"))
                        .Where(c => c != null)
                        .ToList()
                };
            }
            public static GameObjectDataLight BuildDataLight(UnityEngine.GameObject go)
            {
                if (go == null)
                    return null;

                return new GameObjectDataLight()
                {
                    name = go.name,
                    tag = go.tag,
                    layer = go.layer,
                    instanceID = go.GetInstanceID()
                };
            }
            public static string Serialize(UnityEngine.GameObject go)
            {
                if (go == null)
                    return null;

                var data = BuildData(go);
                if (data == null)
                    return null;

                return JsonUtils.JsonSerialize(data);
            }
            public static string SerializeLight(UnityEngine.GameObject go)
            {
                if (go == null)
                    return null;

                var data = BuildDataLight(go);
                if (data == null)
                    return null;

                return JsonUtils.JsonSerialize(data);
            }
        }
    }
}
