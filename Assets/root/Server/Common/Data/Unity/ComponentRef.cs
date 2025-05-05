#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace com.IvanMurzak.Unity.MCP.Common.Data.Unity
{
    [Description(@"Component reference. Used to find Component at GameObject.
Use one of the following properties:
1. 'instanceID' (int) - recommended. It finds the exact Component. Default value is 0.
2. 'index' (int) - finds Component by index. It may find a wrong Component. Default value is -1.
3. 'name' (string) - finds Component by name. It may find a wrong Component. Default value is null.")]
    public class ComponentRef
    {
        [Description("Component 'instanceID' (int). Priority: 1. (Recommended)")]
        public int instanceID { get; set; } = 0;
        [Description("Component 'index'. Priority: 2.")]
        public int index { get; set; } = -1;
        [Description("Component 'type'. Priority: 3. Full name of the component type.")]
        public string? type { get; set; } = null;

        [JsonIgnore]
        public bool IsValid
        {
            get
            {
                if (instanceID != 0)
                    return true;
                if (index >= 0)
                    return true;
                if (!string.IsNullOrEmpty(type))
                    return true;
                return false;
            }
        }

        public ComponentRef() { }

        public override string ToString()
        {
            if (instanceID != 0)
                return $"Component instanceID='{instanceID}'";
            if (index >= 0)
                return $"Component index='{index}'";
            if (!string.IsNullOrEmpty(type))
                return $"Component type='{type}'";
            return "Component unknown";
        }
    }
}