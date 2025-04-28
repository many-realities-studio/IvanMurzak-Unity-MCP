#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using com.IvanMurzak.Unity.MCP.Common;
using com.IvanMurzak.Unity.MCP.Common.Data.Utils;
using UnityEngine;

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
                        fields = SerializeFields(obj, flags),
                        properties = SerializeProperties(obj, flags)
                    }
                    : SerializedMember.FromJson(type, JsonUtils.Serialize(obj), name: name);
            }

            throw new ArgumentException($"Unsupported type: {type.FullName}");
        }
        protected override IEnumerable<FieldInfo> GetSerializableFields(Type objType, BindingFlags flags)
            => objType.GetFields(flags)
                .Where(field => field.GetCustomAttribute<ObsoleteAttribute>() == null)
                .Where(field => field.IsPublic || field.IsPrivate && field.GetCustomAttribute<SerializeField>() != null);

        protected override IEnumerable<PropertyInfo> GetSerializableProperties(Type objType, BindingFlags flags)
            => objType.GetProperties(flags)
                .Where(prop => prop.GetCustomAttribute<ObsoleteAttribute>() == null)
                .Where(prop => prop.CanRead);

        protected override bool SetValue(ref object obj, Type type, JsonElement? value)
        {
            var parsedValue = JsonUtils.Deserialize(value.Value.GetRawText(), type);
            obj = parsedValue;
            return true;
        }
        protected override bool SetFieldValue(ref object obj, Type type, FieldInfo fieldInfo, object? value)
        {
            fieldInfo.SetValue(obj, value);
            return true;
        }
        protected override bool SetPropertyValue(ref object obj, Type type, PropertyInfo propertyInfo, object? value)
        {
            propertyInfo.SetValue(obj, value);
            return true;
        }
    }
}