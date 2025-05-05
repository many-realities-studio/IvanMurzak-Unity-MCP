#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using com.IvanMurzak.Unity.MCP.Common.Data.Utils;

namespace com.IvanMurzak.Unity.MCP.Utils
{
    public static class ExtensionsSerializedMember
    {
        public static int GetInstanceID(this SerializedMember member)
            => member.GetValue<ObjectRef>()?.instanceID
            ?? member.GetField("instanceID")?.GetValue<int>()
            ?? 0;        
    }
}