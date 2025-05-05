#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.IvanMurzak.Unity.MCP.Common;
using com.IvanMurzak.Unity.MCP.Common.Data.Unity;
using UnityEngine;

namespace com.IvanMurzak.Unity.MCP.Utils
{
    public static partial class GameObjectUtils
    {
        public static GameObject FindBy(GameObjectRef gameObjectRef, out string error)
        {
            if (gameObjectRef == null)
            {
                error = "[Error] GameObjectRef is null.";
                return null;
            }
            if (!gameObjectRef.IsValid)
            {
                error = "[Error] GameObjectRef is not valid. At least one of the properties should be set.";
                return null;
            }
            return FindBy(gameObjectRef.instanceID, gameObjectRef.path, gameObjectRef.name, out error);
        }

        public static GameObject FindBy(int? instanceID, string? path, string? name, out string error)
        {
            path = StringUtils.TrimPath(path);
            var go = default(GameObject);

            // Find by 'instanceID' (int). Priority: 1. (Recommended)
            if (instanceID.HasValue && instanceID.Value != 0)
            {
                go = FindByInstanceID(instanceID.Value);
                if (go == null)
                {
                    error = $"[Error] Not found GameObject with instanceID '{instanceID.Value}'";
                    return null;
                }
            }
            // Find by 'path'. Priority: 2.
            else if (!string.IsNullOrEmpty(path))
            {
                go = FindByPath(path);
                if (go == null)
                {
                    error = $"[Error] Not found GameObject at path '{path}'";
                    return null;
                }
            }
            // Find by 'name'. Priority: 3.
            else if (!string.IsNullOrEmpty(name))
            {
                go = GameObject.Find(name);
                if (go == null)
                {
                    error = $"[Error] Not found GameObject with name '{name}'";
                    return null;
                }
            }
            // No valid arguments provided
            else
            {
                error = "[Error] No valid arguments provided to find GameObject.";
                return null;
            }
            error = null;
            return go;
        }

        public static GameObject FindByPath(string path, GameObject? root = null)
        {
            path = StringUtils.TrimPath(path);

            if (string.IsNullOrEmpty(path))
                return null;

            // If root is null, search in the active scene's root GameObjects
            if (root == null)
            {
                var rootGos = FindRootGameObjects();
                var pathParts = path.Split('/');

                root = rootGos.FirstOrDefault(go => go.name == pathParts[0]);
                if (root == null)
                    return null;

                var currentGameObject = root;

                foreach (var part in pathParts.Skip(1))
                {
                    if (currentGameObject == null)
                        return null;

                    currentGameObject = FindChildByName(currentGameObject, part);
                }

                return currentGameObject;
            }
            else
            {
                var pathParts = path.Split('/');
                var currentGameObject = root;

                foreach (var part in pathParts)
                {
                    if (currentGameObject == null)
                        return null;

                    currentGameObject = FindChildByName(currentGameObject, part);
                }

                return currentGameObject;
            }
        }
        public static GameObject FindChildByName(this GameObject parent, string name)
        {
            if (parent == null || string.IsNullOrEmpty(name))
                return null;

            foreach (Transform child in parent.transform)
            {
                if (child.name == name)
                    return child.gameObject;
            }

            return null;
        }
        public static GameObject AddChild(this GameObject parent, string name)
        {
            if (parent == null || string.IsNullOrEmpty(name))
                return null;

            return parent.AddChild(new GameObject(name));
        }
        public static GameObject AddChild(this GameObject parent, GameObject child)
        {
            if (parent == null || child == null)
                return null;

            child.transform.SetParent(parent.transform, false);
            return child;
        }
        public static IEnumerable<KeyValuePair<string, GameObject>> GetAllRecursively(this GameObject root, string path = null)
        {
            var currentPath = string.IsNullOrEmpty(path)
                ? root.name
                : $"{path}/{root.name}";

            yield return new(currentPath, root);

            foreach (Transform child in root.transform)
            {
                foreach (var childContent in GetAllRecursively(child.gameObject, currentPath))
                {
                    yield return childContent;
                }
            }
        }
        public static string GetPath(this GameObject go)
        {
            if (go == null)
                return null;

            var path = new StringBuilder(go.name);
            var currentTransform = go.transform.parent;

            while (currentTransform != null)
            {
                path.Insert(0, '/'); // Prepend '/' to the start
                path.Insert(0, currentTransform.name); // Prepend the name to the start
                currentTransform = currentTransform.parent;
            }

            return path.ToString();
        }
        public static string Print(this GameObject go) => go == null
            ? null
            : $"instanceID: {go.GetInstanceID()}, path: {go.GetPath()}, bounds: {JsonUtils.Serialize(go.CalculateBounds())}";

        public static string Print(this IEnumerable<GameObject> gos)
        {
            var sb = new StringBuilder();
            foreach (var go in gos)
                sb.AppendLine(go.Print());

            return sb.ToString();
        }
        public static GameObjectMetadata ToMetadata(this GameObject go, int includeChildrenDepth = 3)
            => GameObjectMetadata.FromGameObject(go, includeChildrenDepth);

        public static Bounds CalculateBounds(this GameObject go)
        {
            var renderers = go.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
                return new Bounds(go.transform.position, Vector3.zero);

            var bounds = renderers[0].bounds;
            foreach (var renderer in renderers.Skip(1))
            {
                bounds.Encapsulate(renderer.bounds);
            }
            return bounds;
        }
        public static GameObject SetTransform(this GameObject go, Vector3? position = default, Vector3? rotation = default, Vector3? scale = default, bool isLocalSpace = false)
        {
            if (isLocalSpace)
            {
                if (position.HasValue)
                    go.transform.localPosition = position.Value;
                if (rotation.HasValue)
                    go.transform.localRotation = Quaternion.Euler(rotation.Value.x, rotation.Value.y, rotation.Value.z);
                if (scale.HasValue)
                    go.transform.localScale = scale.Value;
            }
            else
            {
                if (position.HasValue)
                    go.transform.position = position.Value;
                if (rotation.HasValue)
                    go.transform.rotation = Quaternion.Euler(rotation.Value.x, rotation.Value.y, rotation.Value.z);
                if (scale.HasValue)
                    go.transform.localScale = scale.Value;
            }
            return go;
        }
    }
}