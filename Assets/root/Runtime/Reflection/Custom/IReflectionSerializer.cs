#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using System.Reflection;
using System.Text;
using com.IvanMurzak.Unity.MCP.Common.Data.Utils;

namespace com.IvanMurzak.Unity.MCP.Utils
{
    public interface IReflectionSerializer
    {
        bool AllowCascadeSerialize { get; }
        bool AllowCascadePopulate { get; }

        int SerializationPriority(Type type);
        int PopulatePriority(Type type);
        SerializedMember Serialize(object obj, Type? type = null, string? name = null, bool recursive = true, BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        StringBuilder? Populate(ref object obj, SerializedMember data, int depth = 0, StringBuilder? stringBuilder = null, BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    }
}