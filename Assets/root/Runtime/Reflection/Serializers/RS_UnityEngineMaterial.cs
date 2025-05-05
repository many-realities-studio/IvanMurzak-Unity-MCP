#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using com.IvanMurzak.Unity.MCP.Common;
using com.IvanMurzak.Unity.MCP.Common.Data.Utils;
using UnityEngine;

namespace com.IvanMurzak.Unity.MCP.Utils
{
    public partial class RS_UnityEngineMaterial : RS_Generic<Material>
    {
        const string FieldShader = "shader";
        const string FieldName = "name";

        protected override SerializedMember InternalSerialize(object obj, Type type, string name = null, bool recursive = true, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var material = obj as Material;
            var shader = material.shader;
            int propertyCount = shader.GetPropertyCount();

            var properties = new List<SerializedMember>(propertyCount);

            for (int i = 0; i < propertyCount; i++)
            {
                var propName = shader.GetPropertyName(i);
                var propType = shader.GetPropertyType(i) switch
                {
                    UnityEngine.Rendering.ShaderPropertyType.Int => typeof(int),
                    UnityEngine.Rendering.ShaderPropertyType.Float => typeof(float),
                    UnityEngine.Rendering.ShaderPropertyType.Range => typeof(float),
                    UnityEngine.Rendering.ShaderPropertyType.Color => typeof(Color),
                    UnityEngine.Rendering.ShaderPropertyType.Vector => typeof(Vector4),
                    UnityEngine.Rendering.ShaderPropertyType.Texture => typeof(Texture),
                    _ => null
                };
                var propValue = shader.GetPropertyType(i) switch
                {
                    UnityEngine.Rendering.ShaderPropertyType.Int => material.GetInt(propName) as object,
                    UnityEngine.Rendering.ShaderPropertyType.Float => material.GetFloat(propName),
                    UnityEngine.Rendering.ShaderPropertyType.Range => material.GetFloat(propName),
                    UnityEngine.Rendering.ShaderPropertyType.Color => material.GetColor(propName),
                    UnityEngine.Rendering.ShaderPropertyType.Vector => material.GetVector(propName),
                    UnityEngine.Rendering.ShaderPropertyType.Texture => material.GetTexture(propName)?.GetInstanceID() != null
                        ? new ObjectRef(material.GetTexture(propName).GetInstanceID())
                        : null,
                    _ => default
                };
                if (propType == null)
                {
                    Debug.LogWarning($"Material property '{propName}' is null or unsupported type '{shader.GetPropertyType(i)}'.");
                    continue;
                }
                properties.Add(SerializedMember.FromValue(propType, propValue, name: propName));
            }

            return new SerializedMember()
            {
                name = name,
                type = type.FullName,
                fields = new List<SerializedMember>()
                {
                    SerializedMember.FromValue(name: FieldName, value: material.name),
                    SerializedMember.FromValue(name: FieldShader, value: shader.name)
                },
                properties = properties,
            }.SetValue(new ObjectRef(material.GetInstanceID()));
        }

        protected override bool SetValue(ref object obj, Type type, JsonElement? value)
        {
            var serialized = JsonUtils.Deserialize<SerializedMember>(value.Value);
            var material = obj as Material;

            // Set shader
            var shaderName = serialized.GetField(FieldShader)?.GetValue<string>();
            if (!string.IsNullOrEmpty(shaderName) && material.shader.name != shaderName)
                material.shader = Shader.Find(shaderName) ?? throw new ArgumentException($"Shader '{shaderName}' not found.");

            return true;
        }

        protected override StringBuilder? ModifyField(ref object obj, SerializedMember fieldValue, StringBuilder? stringBuilder = null, int depth = 0,
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        {
            var material = obj as Material;

            // Set shader
            if (fieldValue.name == FieldShader)
            {
                var shaderName = fieldValue.GetValue<string>();
                if (!string.IsNullOrEmpty(shaderName) && material.shader.name != shaderName)
                {
                    var shader = Shader.Find(shaderName);
                    if (shader == null)
                        return stringBuilder?.AppendLine(new string(' ', depth) + $"[Error] Shader '{shaderName}' not found.");

                    material.shader = shader;
                    return stringBuilder?.AppendLine(new string(' ', depth) + $"[Success] Material '{material.name}' shader set to '{shaderName}'.");
                }
            }

            return stringBuilder;
        }
    }
}