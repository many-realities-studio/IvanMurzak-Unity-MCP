#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System.ComponentModel;
using System.Linq;
using System.Text;
using com.IvanMurzak.Unity.MCP.Common;
using com.IvanMurzak.Unity.MCP.Common.Data.Unity;
using com.IvanMurzak.Unity.MCP.Common.Data.Utils;
using com.IvanMurzak.Unity.MCP.Common.Utils;
using com.IvanMurzak.Unity.MCP.Utils;

namespace com.IvanMurzak.Unity.MCP.Editor.API
{
    public partial class Tool_GameObject
    {
        [McpPluginTool
        (
            "GameObject_Modify",
            Title = "Modify GameObject in opened Prefab or in a Scene"
        )]
        [Description("Modify GameObject and/or attached component's field and properties.")]
        public string Modify
        (
            [Description(@"Json Object with required readonly 'instanceID' and 'type' fields.
Each field and property requires to have 'type' and 'name' fields to identify the exact modification target.
Follow the object schema to specify what to change, ignore values that should not be modified.
Any unknown or wrong located fields and properties will be ignored.
Check the result of this command to see what was changed. The ignored fields and properties will not be listed.")]
            SerializedMember[] values,
            GameObjectRefList gameObjectRefs
        )
        => MainThread.Run(() =>
        {
            if (gameObjectRefs.Count == 0)
                return "[Error] No GameObject references provided. Please provide at least one GameObject reference.";
                
            if (values.Length != gameObjectRefs.Count)
                return $"[Error] The number of {nameof(values)} and {nameof(gameObjectRefs)} should be the same. " +
                    $"{nameof(values)}: {values.Length}, {nameof(gameObjectRefs)}: {gameObjectRefs.Count}";

            var stringBuilder = new StringBuilder();

            for (int i = 0; i < gameObjectRefs.Count; i++)
            {
                var go = GameObjectUtils.FindBy(gameObjectRefs[i], out var error);
                if (error != null)
                {
                    stringBuilder.AppendLine(error);
                    continue;
                }
                var goObj = (object)go;
                Serializer.Populate(ref goObj, values[i], stringBuilder);
            }

            return stringBuilder.ToString();
        });
    }
}