#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using System.Reflection;
using System.Text.Json;
using com.IvanMurzak.Unity.MCP.Common;
using com.IvanMurzak.Unity.MCP.Common.Data.Utils;

namespace com.IvanMurzak.Unity.MCP.Utils
{
    public partial class RS_Generic<T> : ReflectionSerializerBase<T>
    {
        protected override SerializedMember InternalSerialize(object obj, Type type, string name = null, bool recursive = true, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var isStruct = type.IsValueType && !type.IsPrimitive && !type.IsEnum;

            if (type.IsPrimitive ||
                type.IsEnum ||
                type == typeof(string) ||
                type == typeof(decimal) ||
                type == typeof(DateTime))
                return SerializedMember.FromValue(type, obj, name: name);

            if (type.IsClass || isStruct)
            {
                return recursive
                    ? new SerializedMember()
                    {
                        name = name,
                        type = type.FullName,
                        fields = Serializer.Anything.SerializeFields(obj, flags),
                        properties = Serializer.Anything.SerializeProperties(obj, flags)
                    }
                    : SerializedMember.FromJson(type, JsonUtils.Serialize(obj), name: name);
            }

            throw new ArgumentException($"Unsupported type: {type.FullName}");
        }

        protected override bool SetValue(ref object obj, Type type, JsonElement? value)
        {
            var parsedValue = JsonUtils.Deserialize(value.Value.GetRawText(), type);
            obj = parsedValue;
            return true;
        }
        protected override bool SetFieldValue(ref object obj, Type type, FieldInfo fieldInfo, JsonElement? value)
        {
            var parsedValue = JsonUtils.Deserialize(value.Value.GetRawText(), type);
            fieldInfo.SetValue(obj, parsedValue);
            return true;
        }
        protected override bool SetPropertyValue(ref object obj, Type type, PropertyInfo propertyInfo, JsonElement? value)
        {
            var parsedValue = JsonUtils.Deserialize(value.Value.GetRawText(), type);
            propertyInfo.SetValue(obj, parsedValue);
            return true;
        }
    }
}