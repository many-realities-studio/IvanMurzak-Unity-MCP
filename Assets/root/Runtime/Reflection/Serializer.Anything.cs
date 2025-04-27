#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using com.IvanMurzak.Unity.MCP.Common;
using com.IvanMurzak.Unity.MCP.Common.Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace com.IvanMurzak.Unity.MCP.Utils
{
    /// <summary>
    /// Serializes Unity components to JSON format.
    /// </summary>
    public static partial class Serializer
    {
        public static class Anything
        {
            public static SerializedMember Serialize(object obj, Type? type = null, string? name = null, bool recursive = true,
                BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            {
                name ??= string.Empty;
                type ??= obj?.GetType();

                if (obj == null)
                    return SerializedMember.FromJson(type, null, name);

                var isStruct = type.IsValueType && !type.IsPrimitive && !type.IsEnum;

                if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal) || type == typeof(DateTime))
                {
                    // Handle as primitive type
                    return SerializedMember.FromPrimitive(type, obj, name);
                }
                if (type.IsEnum)
                {
                    // Handle as enum type
                    return SerializedMember.FromJson(type, JsonUtils.Serialize(obj), name);
                }
                if (type.IsClass)
                {
                    // Handle as class type
                    if (recursive)
                    {
                        return new SerializedMember()
                        {
                            type = type.FullName,
                            name = name,
                            fields = SerializeFields(obj, flags),
                            properties = SerializeProperties(obj, flags)
                        };
                    }
                    else
                    {
                        var isUnityObject = typeof(UnityEngine.Object).IsAssignableFrom(type);
                        if (isUnityObject)
                        {
                            var unityObject = obj as UnityEngine.Object;
                            var instanceIDJson = JsonUtility.ToJson(new InstanceID(unityObject.GetInstanceID()));
                            return SerializedMember.FromJson(type, instanceIDJson, name);
                        }
                        return SerializedMember.FromJson(type, JsonUtility.ToJson(obj), name);
                    }
                }
                if (isStruct)
                {
                    // Handle as struct type
                    return SerializedMember.FromJson(type, JsonUtility.ToJson(obj), name);
                }

                throw new ArgumentException($"Unsupported type: {type.FullName}");
            }

            public static List<SerializedMember> SerializeFields(object obj, BindingFlags flags)
            {
                var serialized = default(List<SerializedMember>);
                var objType = obj.GetType();

                foreach (var field in objType.GetFields(flags)
                    .Where(field => field.GetCustomAttribute<ObsoleteAttribute>() == null)
                    .Where(field => field.IsPublic || field.IsPrivate && field.GetCustomAttribute<SerializeField>() != null))
                {
                    var value = field.GetValue(obj);
                    var fieldType = field.FieldType;

                    serialized ??= new();
                    serialized.Add(Serialize(value, fieldType, name: field.Name, recursive: false, flags: flags));
                }
                return serialized;
            }

            public static List<SerializedMember> SerializeProperties(object obj, BindingFlags flags)
            {
                var serialized = default(List<SerializedMember>);
                var objType = obj.GetType();

                foreach (var prop in objType.GetProperties(flags)
                    .Where(prop => prop.GetCustomAttribute<ObsoleteAttribute>() == null)
                    .Where(prop => prop.CanRead))
                {
                    try
                    {
                        var value = prop.GetValue(obj);
                        var propType = prop.PropertyType;

                        serialized ??= new();
                        serialized.Add(Serialize(value, propType, name: prop.Name, recursive: false, flags: flags));
                    }
                    catch { /* skip inaccessible properties */ }
                }
                return serialized;
            }
        }
    }
}
