using System.Collections;
using com.IvanMurzak.Unity.MCP.Common.Data.Utils;
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
        public IEnumerator ModifyComponent_Vector3()
        {
            var child = new GameObject(GO_ParentName).AddChild(GO_ChildName);
            var newPosition = new Vector3(1, 2, 3);

            var data = SerializedMember.FromValue(name: nameof(child.transform),
                    type: typeof(Transform),
                    value: new InstanceID(child.transform.GetInstanceID())
                )
                .AddProperty(SerializedMember.FromValue(name: nameof(child.transform.position),
                    value: newPosition));

            var result = new Tool_GameObject().ModifyComponent(data, instanceID: child.GetInstanceID());

            Debug.Log($"[{nameof(TestToolGameObject)}] Result:\n{result}");

            Assert.IsNotNull(result, $"Result should not be null");
            Assert.IsTrue(result.Contains(GO_ChildName), $"{GO_ChildName} should be found in the path");
            Assert.IsFalse(result.ToLower().Contains("error"), $"{GO_ChildName} should not contain 'error' in the path");

            Assert.AreEqual(child.transform.position, newPosition, "Position should be changed");
            Assert.AreEqual(child.transform.GetInstanceID(), data.GetInstanceID(), "InstanceID should be the same");

            yield return null;
        }
    }
}