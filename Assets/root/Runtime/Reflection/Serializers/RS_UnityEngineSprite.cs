#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using System.Reflection;
using System.Text;
using com.IvanMurzak.Unity.MCP.Common.Data.Utils;

namespace com.IvanMurzak.Unity.MCP.Utils
{
    public partial class RS_UnityEngineSprite : RS_UnityEngineObject<UnityEngine.Sprite>
    {
        protected override SerializedMember InternalSerialize(object obj, Type type, string name = null, bool recursive = true, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            if (obj is UnityEngine.Texture2D texture)
            {
                var objectRef = new ObjectRef(texture.GetInstanceID());
                return SerializedMember.FromValue(type, objectRef, name);
            }

            return base.InternalSerialize(obj, type, name, recursive, flags);
        }
        public override bool SetAsField(ref object obj, Type type, FieldInfo fieldInfo, SerializedMember? value, StringBuilder? stringBuilder = null,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var currentValue = fieldInfo.GetValue(obj);
            Populate(ref currentValue, value, 0, null, flags);
            fieldInfo.SetValue(obj, currentValue);
            stringBuilder?.AppendLine($"[Success] Field '{value.name}' modified to '{currentValue}'.");
            return true;
        }
        public override bool SetAsProperty(ref object obj, Type type, PropertyInfo propertyInfo, SerializedMember? value, StringBuilder? stringBuilder = null,
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var currentValue = propertyInfo.GetValue(obj);
            Populate(ref currentValue, value, 0, null, flags);
            propertyInfo.SetValue(obj, currentValue);
            stringBuilder?.AppendLine($"[Success] Property '{value.name}' modified to '{currentValue}'.");
            return true;
        }
    }
}