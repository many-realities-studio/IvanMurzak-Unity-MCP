#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System.Collections.Generic;
using System.ComponentModel;

namespace com.IvanMurzak.Unity.MCP.Common.Data.Unity
{
    [Description(@"GameObject references. Used to specify GameObjects in opened Prefab or in a Scene.
Use one of the following properties:
1. 'instanceID' (int) - recommended. It finds the exact GameObject.
2. 'path' (string) - finds GameObject by path. It may find a wrong GameObject.
3. 'name' (string) - finds GameObject by name. It may find a wrong GameObject.")]
    public class GameObjectRefList : List<GameObjectRef>
    {
        public GameObjectRefList() { }

        public GameObjectRefList(int capacity) : base(capacity) { }

        public GameObjectRefList(IEnumerable<GameObjectRef> collection) : base(collection) { }

        public override string ToString()
        {
            if (Count == 0)
                return "No GameObjects";

            var stringBuilder = new System.Text.StringBuilder();

            stringBuilder.AppendLine($"GameObjects total amount: {Count}");
            
            for (int i = 0; i < Count; i++)
                stringBuilder.AppendLine($"GameObject[{i}] {this[i]}");
                
            return stringBuilder.ToString();
        }
    }
}