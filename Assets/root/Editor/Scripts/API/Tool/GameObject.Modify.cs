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
            Title = "Modify GameObjects in opened Prefab or in a Scene"
        )]
        [Description(@"Modify GameObjects and/or attached component's field and properties.
You can modify multiple GameObjects at once. Just provide the same number of GameObject references and SerializedMember objects.")]
        public string Modify
        (
            [Description(@"Json Object with required readonly 'instanceID' and 'type' fields.
Each field and property requires to have 'type' and 'name' fields to identify the exact modification target.
Follow the object schema to specify what to change, ignore values that should not be modified. Keep the original data structure.
Any unknown or wrong located fields and properties will be ignored.
Check the result of this command to see what was changed. The ignored fields and properties will be listed.")]
            SerializedMember[] gameObjectDiffs,
            GameObjectRefList gameObjectRefs
        )
        => MainThread.Run(() =>
        {
            if (gameObjectRefs.Count == 0)
                return "[Error] No GameObject references provided. Please provide at least one GameObject reference.";

            if (gameObjectDiffs.Length != gameObjectRefs.Count)
                return $"[Error] The number of {nameof(gameObjectDiffs)} and {nameof(gameObjectRefs)} should be the same. " +
                    $"{nameof(gameObjectDiffs)}: {gameObjectDiffs.Length}, {nameof(gameObjectRefs)}: {gameObjectRefs.Count}";

            var stringBuilder = new StringBuilder();

            for (int i = 0; i < gameObjectRefs.Count; i++)
            {
                var go = GameObjectUtils.FindBy(gameObjectRefs[i], out var error);
                if (error != null)
                {
                    stringBuilder.AppendLine(error);
                    continue;
                }
                var objToModify = (object)go;
                var type = TypeUtils.GetType(gameObjectDiffs[i].type);
                if (typeof(UnityEngine.Component).IsAssignableFrom(type))
                {
                    var component = go.GetComponent(type);
                    if (component == null)
                    {
                        stringBuilder.AppendLine($"[Error] Component '{type.FullName}' not found on GameObject '{go.name}'.");
                        continue;
                    }
                    objToModify = component;
                }
                Serializer.Populate(ref objToModify, gameObjectDiffs[i], stringBuilder);
            }

            return stringBuilder.ToString();
        });
    }
}