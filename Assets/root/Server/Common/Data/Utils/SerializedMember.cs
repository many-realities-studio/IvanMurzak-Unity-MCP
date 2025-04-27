#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace com.IvanMurzak.Unity.MCP.Common.Data.Utils
{
    [Serializable]
    public class SerializedMember
    {
        [JsonInclude] public string? name = string.Empty; // needed for Unity's JsonUtility serialization
        [JsonInclude] public string type = string.Empty; // needed for Unity's JsonUtility serialization
        [JsonInclude] public List<SerializedMember>? fields; // needed for Unity's JsonUtility serialization
        [JsonInclude] public List<SerializedMember>? properties; // needed for Unity's JsonUtility serialization

        [JsonInclude, JsonPropertyName("value")]
        public JsonElement? valueJsonElement = null; // System.Text.Json serialization

        public SerializedMember() { }

        protected SerializedMember(Type type, string? name = null)
        {
            this.name = name;
            this.type = type.FullName ?? throw new ArgumentNullException(nameof(type));
        }

        public SerializedMember SetName(string? name)
        {
            this.name = name;
            return this;
        }

        public SerializedMember? GetField(string name)
            => fields?.FirstOrDefault(x => x.name == name);

        public SerializedMember? GetProperty(string name)
            => properties?.FirstOrDefault(x => x.name == name);

        public T? GetValue<T>()
        {
            if (valueJsonElement == null)
                return default;
            var json = valueJsonElement.Value.GetRawText();
            return JsonSerializer.Deserialize<T>(json) ?? default;
        }

        public static SerializedMember FromJson(Type type, string json, string? name = null)
        {
            var result = new SerializedMember(type, name);
            using (var doc = JsonDocument.Parse(json))
            {
                result.valueJsonElement = doc.RootElement.Clone();
            }
            return result;
        }
        public static SerializedMember FromPrimitive(Type type, object primitiveValue, string? name = null)
        {
            var result = new SerializedMember(type, name);
            var json = JsonSerializer.Serialize(primitiveValue);
            using (var doc = JsonDocument.Parse(json))
            {
                result.valueJsonElement = doc.RootElement.Clone();
            }
            return result;
        }
    }
}