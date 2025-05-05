#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System;
using com.IvanMurzak.Unity.MCP.Common.Utils;

namespace com.IvanMurzak.Unity.MCP.Utils
{
    public abstract partial class RS_Base<T> : IReflectionSerializer
    {
        protected const int MAX_DEPTH = 10000;

        public virtual bool AllowCascadeSerialize => false;
        public virtual bool AllowCascadePopulate => false;

        public virtual int SerializationPriority(Type type)
        {
            if (type == typeof(T))
                return MAX_DEPTH + 1;

            var distance = TypeUtils.GetInheritanceDistance(baseType: typeof(T), targetType: type);

            return distance >= 0
                ? MAX_DEPTH - distance
                : 0; ;
        }
    }
}