#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using System.Reflection;
using System.Text;
using System.Text.Json;
using com.IvanMurzak.Unity.MCP.Common.Data.Utils;
using com.IvanMurzak.Unity.MCP.Common.Utils;

namespace com.IvanMurzak.Unity.MCP.Utils
{
    public abstract class ReflectionSerializerBase<T> : IReflectionSerializer
    {
        const int MAX_DEPTH = 10000;
        public virtual bool AllowCascadeSerialize => false;
        public virtual bool AllowCascadePopulate => false;

        public virtual int SerializationPriority(Type type)
        {
            var distance = TypeUtils.GetInheritanceDistance(baseType: typeof(T), targetType: type);
            return distance == -1 ? 0 : MAX_DEPTH - distance;
        }
        public virtual int PopulatePriority(Type type)
        {
            var distance = TypeUtils.GetInheritanceDistance(baseType: typeof(T), targetType: type);
            return distance == -1 ? 0 : MAX_DEPTH - distance;
        }

        public virtual SerializedMember Serialize(object obj, Type? type = null, string? name = null, bool recursive = true,
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            type ??= obj?.GetType() ?? typeof(T);

            if (obj == null)
                return SerializedMember.FromJson(type, json: null, name: name);

            return InternalSerialize(obj, type, name, recursive, flags);
        }

        protected abstract SerializedMember InternalSerialize(object obj, Type type, string? name = null, bool recursive = true,
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        public virtual StringBuilder? Populate(ref object obj, SerializedMember data, int depth = 0, StringBuilder? stringBuilder = null, BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            if (string.IsNullOrEmpty(data.type))
                return stringBuilder?.AppendLine("[Error] Type is null or empty.");

            var type = TypeUtils.GetType(data.type);
            if (type == null)
                return stringBuilder?.AppendLine($"[Error] Type not found: {data.type}");

            var castedComponent = TypeUtils.CastTo(obj, data.type, out var error);
            if (error != null)
                return stringBuilder?.AppendLine(error);

            if (!type.IsAssignableFrom(obj.GetType()))
                return stringBuilder?.AppendLine($"[Error] Type mismatch: {data.type} vs {obj.GetType().FullName}");

            if (data.valueJsonElement != null)
            {
                try
                {
                    SetValue(ref obj, type, data.valueJsonElement);
                    stringBuilder?.AppendLine(new string(' ', depth) + $"[Success] Object '{obj}' modified to '{data.valueJsonElement}'.");
                }
                catch (Exception ex)
                {
                    stringBuilder?.AppendLine(new string(' ', depth) + $"[Error] Object '{obj}' modification failed: {ex.Message}");
                }
            }

            var nextDepth = depth + 1;

            if (data.fields != null)
                foreach (var field in data.fields)
                    ModifyField(ref castedComponent, field, stringBuilder, depth: nextDepth, flags: flags);

            if (data.properties != null)
                foreach (var property in data.properties)
                    ModifyProperty(ref castedComponent, property, stringBuilder, depth: nextDepth, flags: flags);

            return stringBuilder;
        }
        protected abstract bool SetValue(ref object obj, Type type, JsonElement? value);

        protected virtual StringBuilder? ModifyField(ref object obj, SerializedMember fieldValue, StringBuilder? stringBuilder = null, int depth = 0,
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            if (string.IsNullOrEmpty(fieldValue.name))
                return stringBuilder?.AppendLine(new string(' ', depth) + Serializer.Error.ComponentFieldNameIsEmpty());

            if (string.IsNullOrEmpty(fieldValue.type))
                return stringBuilder?.AppendLine(new string(' ', depth) + Serializer.Error.ComponentFieldTypeIsEmpty());

            var fieldInfo = obj.GetType().GetField(fieldValue.name, flags);
            if (fieldInfo == null)
                return stringBuilder?.AppendLine(new string(' ', depth) + $"[Error] Field '{fieldValue.name}' not found. Can't modify field '{fieldValue.name}'.");

            var targetType = TypeUtils.GetType(fieldValue.type);
            if (targetType == null)
                return stringBuilder?.AppendLine(new string(' ', depth) + Serializer.Error.InvalidComponentFieldType(fieldValue, fieldInfo));

            try
            {
                SetFieldValue(ref obj, targetType, fieldInfo, fieldValue.valueJsonElement);
                return stringBuilder?.AppendLine(new string(' ', depth) + $"[Success] Field '{fieldValue.name}' modified to '{fieldValue.valueJsonElement}'.");
            }
            catch (Exception ex)
            {
                var message = $"[Error] Field '{fieldValue.name}' modification failed: {ex.Message}";
                return stringBuilder?.AppendLine(new string(' ', depth) + message);
            }
        }
        protected abstract bool SetFieldValue(ref object obj, Type type, FieldInfo propertyInfo, JsonElement? value);

        protected virtual StringBuilder? ModifyProperty(ref object obj, SerializedMember propertyValue, StringBuilder? stringBuilder = null, int depth = 0,
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            if (string.IsNullOrEmpty(propertyValue.name))
                return stringBuilder?.AppendLine(new string(' ', depth) + Serializer.Error.ComponentPropertyNameIsEmpty());

            if (string.IsNullOrEmpty(propertyValue.type))
                return stringBuilder?.AppendLine(new string(' ', depth) + Serializer.Error.ComponentPropertyTypeIsEmpty());

            var propInfo = obj.GetType().GetProperty(propertyValue.name, flags);
            if (propInfo == null)
            {
                var warningMessage = $"[Error] Property '{propertyValue.name}' not found. Can't modify property '{propertyValue.name}'.";
                return stringBuilder?.AppendLine(new string(' ', depth) + warningMessage);
            }
            if (!propInfo.CanWrite)
            {
                var warningMessage = $"[Error] Property '{propertyValue.name}' is not writable. Can't modify property '{propertyValue.name}'.";
                return stringBuilder?.AppendLine(new string(' ', depth) + warningMessage);
            }

            var targetType = TypeUtils.GetType(propertyValue.type);
            if (targetType == null)
                return stringBuilder?.AppendLine(new string(' ', depth) + Serializer.Error.InvalidComponentPropertyType(propertyValue, propInfo));

            try
            {
                SetPropertyValue(ref obj, targetType, propInfo, propertyValue.valueJsonElement);
                return stringBuilder?.AppendLine(new string(' ', depth) + $"[Success] Property '{propertyValue.name}' modified to '{propertyValue.valueJsonElement}'.");
            }
            catch (Exception ex)
            {
                var message = $"[Error] Property '{propertyValue.name}' modification failed: {ex.Message}";
                return stringBuilder?.AppendLine(new string(' ', depth) + message);
            }
        }
        protected abstract bool SetPropertyValue(ref object obj, Type type, PropertyInfo propertyInfo, JsonElement? value);
    }
}