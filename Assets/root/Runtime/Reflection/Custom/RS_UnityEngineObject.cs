#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using System.Reflection;
using System.Text.Json;
using com.IvanMurzak.Unity.MCP.Common;
using com.IvanMurzak.Unity.MCP.Common.Data.Utils;

namespace com.IvanMurzak.Unity.MCP.Utils
{
    public partial class RS_UnityEngineObject : RS_Generic<UnityEngine.Object>
    {
        protected override SerializedMember InternalSerialize(object obj, Type type, string name = null, bool recursive = true, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var unityObject = obj as UnityEngine.Object;
            if (type.IsClass)
            {
                if (recursive)
                {
                    return new SerializedMember()
                    {
                        name = name,
                        type = type.FullName,
                        fields = SerializeFields(obj, flags),
                        properties = SerializeProperties(obj, flags)
                    }.SetValue(new InstanceID(unityObject.GetInstanceID()));
                }
                else
                {
                    var instanceID = new InstanceID(unityObject.GetInstanceID());
                    return SerializedMember.FromValue(type, instanceID, name);
                }
            }

            throw new ArgumentException($"Unsupported type: {type.FullName}");
        }

        protected override bool SetValue(ref object obj, Type type, JsonElement? value)
        {
            return true;
        }

        public override bool SetAsField(ref object obj, Type type, FieldInfo fieldInfo, JsonElement? value,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var instanceID = JsonUtils.Deserialize<InstanceID>(value.Value.GetRawText());
            var refObj = UnityEditor.EditorUtility.InstanceIDToObject(instanceID.instanceID);

            fieldInfo.SetValue(obj, refObj);
            return true;
        }

        public override bool SetAsProperty(ref object obj, Type type, PropertyInfo propertyInfo, JsonElement? value,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var instanceID = JsonUtils.Deserialize<InstanceID>(value.Value.GetRawText());
            var refObj = UnityEditor.EditorUtility.InstanceIDToObject(instanceID.instanceID);

            propertyInfo.SetValue(obj, refObj);
            return true;
        }
    }
}