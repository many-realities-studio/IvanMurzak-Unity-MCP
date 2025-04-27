using System.Collections;
using com.IvanMurzak.Unity.MCP.Editor.API;
using com.IvanMurzak.Unity.MCP.Utils;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace com.IvanMurzak.Unity.MCP.Editor.Tests
{
    public partial class TestToolGameObject
    {
        [UnityTest]
        public IEnumerator FindByInstanceId()
        {
            var child = new GameObject(GO_ParentName).AddChild(GO_ChildName);

            var result = new Tool_GameObject().Find(instanceID: child.GetInstanceID());

            Debug.Log($"[{nameof(TestToolGameObject)}] Result:\n{result}");

            Assert.IsNotNull(result, $"Result should not be null");
            Assert.IsTrue(result.Contains(GO_ChildName), $"{GO_ChildName} should be found in the path");
            Assert.IsFalse(result.ToLower().Contains("error"), $"{GO_ChildName} should not contain 'error' in the path");

            yield return null;
        }

        [UnityTest]
        public IEnumerator FindByPath()
        {
            var child = new GameObject(GO_ParentName).AddChild(GO_ChildName);

            var result = new Tool_GameObject().Find(path: $"{GO_ParentName}/{GO_ChildName}");

            Debug.Log($"[{nameof(TestToolGameObject)}] Result:\n{result}");

            Assert.IsNotNull(result, $"Result should not be null");
            Assert.IsTrue(result.Contains(GO_ChildName), $"{GO_ChildName} should be found in the path");
            Assert.IsFalse(result.ToLower().Contains("error"), $"{GO_ChildName} should not contain 'error' in the path");

            yield return null;
        }

        [UnityTest]
        public IEnumerator FindByName()
        {
            var child = new GameObject(GO_ParentName).AddChild(GO_ChildName);

            var result = new Tool_GameObject().Find(name: GO_ChildName);

            Debug.Log($"[{nameof(TestToolGameObject)}] Result:\n{result}");

            Assert.IsNotNull(result, $"Result should not be null");
            Assert.IsTrue(result.Contains(GO_ChildName), $"{GO_ChildName} should be found in the path");
            Assert.IsFalse(result.ToLower().Contains("error"), $"{GO_ChildName} should not contain 'error' in the path");

            yield return null;
        }
    }
}