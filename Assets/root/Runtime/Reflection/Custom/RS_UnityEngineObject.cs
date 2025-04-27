#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using System.Reflection;
using System.Text.Json;
using com.IvanMurzak.Unity.MCP.Common;

namespace com.IvanMurzak.Unity.MCP.Utils
{
    public partial class RS_UnityEngineObject : RS_Generic<UnityEngine.Object>
    {
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