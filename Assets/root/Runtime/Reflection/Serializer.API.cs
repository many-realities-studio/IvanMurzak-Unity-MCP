#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using com.IvanMurzak.Unity.MCP.Common.Data.Utils;
using com.IvanMurzak.Unity.MCP.Common.Utils;
using UnityEngine;

namespace com.IvanMurzak.Unity.MCP.Utils
{
    /// <summary>
    /// Serializes Unity components to JSON format.
    /// </summary>
    public static partial class Serializer
    {
        public static SerializedMember Serialize(object obj, Type? type = null, string? name = null, bool recursive = true,
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            type ??= obj?.GetType();

            if (obj == null)
                return SerializedMember.FromJson(type, null);

            foreach (var serializer in Registry.BuildSerializersChain(type))
            {
                UnityEngine.Debug.Log($"[Serializer] {serializer.GetType().Name} for type {type?.FullName}");
                var serializedMember = serializer.Serialize(obj, type: type, name: name, recursive, flags);
                if (serializedMember != null)
                    return serializedMember;
            }
            throw new ArgumentException($"[Error] Type '{type?.FullName}' not supported for serialization.");
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
        public static StringBuilder Populate<T>(ref T obj, SerializedMember data, StringBuilder stringBuilder = null, int depth = 0,
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            stringBuilder ??= new StringBuilder();

            if (string.IsNullOrEmpty(data?.type))
                return stringBuilder.AppendLine(new string(' ', depth) + Error.DataTypeIsEmpty());

            var type = TypeUtils.GetType(data.type);
            if (type == null)
                return stringBuilder.AppendLine(new string(' ', depth) + Error.NotFoundType(data.type));

            if (obj == null)
                return stringBuilder.AppendLine(new string(' ', depth) + Error.TargetObjectIsNull());

            var castedObj = TypeUtils.CastTo(obj, data.type, out var error);
            if (error != null)
                return stringBuilder.AppendLine(new string(' ', depth) + error);

            if (!type.IsAssignableFrom(castedObj.GetType()))
                return stringBuilder.AppendLine(new string(' ', depth) + Error.TypeMismatch(data.type, obj.GetType().FullName));

            var castedObject = (object)obj;

            foreach (var populators in Registry.BuildPopulatorsChain(type))
                populators.Populate(ref castedObject, data, stringBuilder: stringBuilder, flags: flags);

            return stringBuilder;
        }
    }
}
