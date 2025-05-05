using System.Linq;
using com.IvanMurzak.Unity.MCP.Common;
using com.IvanMurzak.Unity.MCP.Common.Data;
using com.IvanMurzak.Unity.MCP.Editor.Utils;
using com.IvanMurzak.Unity.MCP.Utils;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace com.IvanMurzak.Unity.MCP.Editor.API
{
    [McpPluginResourceType]
    public partial class Resource_GameObject
    {
        [McpPluginResource
        (
            Route = "gameObject://currentScene/{path}",
            MimeType = Consts.MimeType.TextJson,
            ListResources = nameof(CurrentSceneAll),
            Name = "GameObject_CurrentScene",
            Description = "Get gameObject's components and the values of each explicit property."
        )]
        public ResponseResourceContent[] CurrentScene(string uri, string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new System.Exception("[Error] Path to the GameObject is empty.");

            // if (path == Consts.AllRecursive)
            // {
            // }
            // if (path == Consts.All)
            // {
            // }

            return MainThread.Run(() =>
            {
                var go = GameObject.Find(path);
                if (go == null)
                    throw new System.Exception($"[Error] GameObject '{path}' not found.");

                return ResponseResourceContent.CreateText(uri, JsonUtils.Serialize(Serializer.Serialize(go)), Consts.MimeType.TextJson).MakeArray();
            });
        }

        public ResponseListResource[] CurrentSceneAll() => MainThread.Run(()
            => EditorSceneManager.GetActiveScene().GetRootGameObjects()
                .SelectMany(root => GameObjectUtils.GetAllRecursively(root))
                .Select(kvp => new ResponseListResource($"gameObject://currentScene/{kvp.Key}", kvp.Value.name, Consts.MimeType.TextJson))
                .ToArray());
    }
}