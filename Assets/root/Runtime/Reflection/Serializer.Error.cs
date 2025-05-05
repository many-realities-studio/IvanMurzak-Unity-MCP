#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using System.Reflection;
using com.IvanMurzak.Unity.MCP.Common.Data.Utils;

namespace com.IvanMurzak.Unity.MCP.Utils
{
    /// <summary>
    /// Serializes Unity components to JSON format.
    /// </summary>
    public static partial class Serializer
    {
        public static class Error
        {
            public static string DataTypeIsEmpty()
                => "[Error] Data type is empty.";
            public static string NotFoundAsset(string assetPath, string assetGuid)
                => $"[Error] Asset not found. Path: {assetPath}. GUID: {assetGuid}.";

            public static string NotAllowedToModifyAssetInPackages(string assetPath)
                => $"[Error] Not allowed to modify asset in '/Packages' folder. Path: {assetPath}.";

            public static string NeitherProvided_AssetPath_AssetGuid()
                => "[Error] Neither 'assetPath' nor 'assetGuid' provided.";

            public static string NotFoundType(string typeFullName)
                => $"[Error] Type '{typeFullName}' not found.";

            public static string TargetObjectIsNull()
                => "[Error] Target object is null.";

            public static string TypeMismatch(string expectedType, string objType)
                => $"[Error] Type mismatch between '{expectedType}' (expected) and '{objType}'.";

            public static string ComponentFieldNameIsEmpty()
                => "[Error] Component field name is empty.";
            public static string ComponentFieldTypeIsEmpty()
                => "[Error] Component field type is empty.";
            public static string ComponentPropertyNameIsEmpty()
                => $"[Error] Component property name is empty. It should be a valid property name.";
            public static string ComponentPropertyTypeIsEmpty()
                => $"[Error] Component property type is empty. It should be a valid property type.";

            public static string InvalidInstanceID(Type holderType, string fieldName)
                => $"[Error] Invalid instanceID '{fieldName}' for '{holderType.FullName}'. It should be a valid field name.";
            public static string InvalidComponentPropertyType(SerializedMember serializedProperty, PropertyInfo propertyInfo)
                => $"[Error] Invalid component property type '{serializedProperty.type}' for '{propertyInfo.Name}'. Expected '{propertyInfo.PropertyType.FullName}'.";
            public static string InvalidComponentFieldType(SerializedMember serializedProperty, FieldInfo propertyInfo)
                => $"[Error] Invalid component property type '{serializedProperty.type}' for '{propertyInfo.Name}'. Expected '{propertyInfo.FieldType.FullName}'.";

            public static string NotSupportedInRuntime(Type type)
                => $"[Error] Type '{type.FullName}' is not supported in runtime for now.";
        }
    }
}
