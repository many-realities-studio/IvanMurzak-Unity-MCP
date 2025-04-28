#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
namespace com.IvanMurzak.Unity.MCP.Utils
{
    public abstract partial class ReflectionSerializerBase<T> : IReflectionSerializer
    {
        const int MAX_DEPTH = 10000;
        public virtual bool AllowCascadeSerialize => false;
        public virtual bool AllowCascadePopulate => false;
    }
}