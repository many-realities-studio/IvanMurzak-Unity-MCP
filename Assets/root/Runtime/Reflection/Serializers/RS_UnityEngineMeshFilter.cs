#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

using System.Collections.Generic;
using System.Linq;

namespace com.IvanMurzak.Unity.MCP.Utils
{
    public partial class RS_UnityEngineMeshFilter : RS_UnityEngineObject<UnityEngine.MeshFilter>
    {
        protected override IEnumerable<string> ignoredProperties => base.ignoredProperties
            .Concat(new[]
            {
                nameof(UnityEngine.MeshFilter.mesh)
            });
    }
}