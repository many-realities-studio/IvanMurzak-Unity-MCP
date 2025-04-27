#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using System.Reflection;
using System.Text.Json;
using com.IvanMurzak.Unity.MCP.Common;
using com.IvanMurzak.Unity.MCP.Common.Data.Utils;
using UnityEngine;

namespace com.IvanMurzak.Unity.MCP.Utils
{
    public partial class RS_UnityEngineObject : RS_Generic<UnityEngine.Object>
    {
        protected override SerializedMember InternalSerialize(object obj, Type type, string name = null, bool recursive = true, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var isStruct = type.IsValueType && !type.IsPrimitive && !type.IsEnum;
            if (isStruct)
                return SerializedMember.FromJson(type, JsonUtility.ToJson(obj), name);

            var unityObject = obj as UnityEngine.Object;
            if (type.IsClass)
            {
                if (recursive)
                {
                    return new SerializedMember()
                    {
                        name = name,
                        type = type.FullName,
                        fields = Serializer.SerializeFields(obj, flags),
                        properties = Serializer.SerializeProperties(obj, flags)
                    }.SetValue(new InstanceID(unityObject.GetInstanceID()));
                }
                else
                {
                    var instanceIDJson = JsonUtility.ToJson(new InstanceID(unityObject.GetInstanceID()));
                    return SerializedMember.FromJson(type, instanceIDJson, name);
                }
            }

            throw new ArgumentException($"Unsupported type: {type.FullName}");
        }

        protected override bool SetValue(ref object obj, Type type, JsonElement? value)
        {
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