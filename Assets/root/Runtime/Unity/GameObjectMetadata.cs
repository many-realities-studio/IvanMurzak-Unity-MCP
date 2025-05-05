#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System.Collections.Generic;
using System.Text;
using com.IvanMurzak.Unity.MCP.Common;
using com.IvanMurzak.Unity.MCP.Utils;
using UnityEngine;

namespace com.IvanMurzak.Unity.MCP
{
    public class GameObjectMetadata
    {
        public int instanceID;
        public string path;
        public string name;
        public string sceneName;
        public string tag;
        public bool activeSelf;
        public bool activeInHierarchy;
        public List<GameObjectMetadata> children = new();

        public string Print(int limit = Consts.MCP.LinesLimit)
        {
            var sb = new StringBuilder();

            // Add table header
            sb.AppendLine("Scene: " + sceneName);
            sb.AppendLine("Path to root: " + path);
            sb.AppendLine("-------------------------------------------------------------------------");
            sb.AppendLine("instanceID | activeInHierarchy | activeSelf | tag       | name");
            sb.AppendLine("-----------|-------------------|------------|-----------|----------------");

            // Add the current GameObject's metadata
            AppendMetadata(sb, this, depth: 0, ref limit);

            return sb.ToString();
        }

        public static void AppendMetadata(StringBuilder sb, GameObjectMetadata metadata, int depth, ref int limit)
        {
            if (limit <= 0)
            {
                sb.AppendLine(new string(' ', depth * 2) + "... [Limit reached] ...");
                return;
            }
            limit--;
            // Indent the path based on depth for better readability
            var indentedPath = new string(' ', depth * 2) + metadata.name;

            // Add the current GameObject's data
            sb.AppendLine($"{metadata.instanceID,-10} | {metadata.activeInHierarchy,-17} | {metadata.activeSelf,-10} | {metadata.tag,-9} | {indentedPath}");

            // Recursively add children
            foreach (var child in metadata.children)
            {
                if (limit <= 0)
                {
                    sb.AppendLine(new string(' ', (depth + 1) * 2) + "... [Limit reached] ...");
                    return;
                }
                limit--;
                AppendMetadata(sb, child, depth + 1, ref limit);
            }
        }

        public static GameObjectMetadata FromGameObject(GameObject go, int includeChildrenDepth = 3)
        {
            if (go == null)
                return null;

            // Create metadata for the GameObject
            var metadata = new GameObjectMetadata
            {
                instanceID = go.GetInstanceID(),
                path = go.GetPath(),
                name = go.name,
                sceneName = go.scene.name,
                tag = go.tag,
                activeSelf = go.activeSelf,
                activeInHierarchy = go.activeInHierarchy
            };

            if (includeChildrenDepth > 0)
            {
                metadata.children ??= new();
                foreach (Transform child in go.transform)
                {
                    var childMetadata = FromGameObject(child.gameObject, includeChildrenDepth - 1);
                    metadata.children.Add(childMetadata);
                }
            }

            return metadata;
        }
    }
}