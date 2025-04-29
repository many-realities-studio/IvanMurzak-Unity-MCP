using System.Collections;
using com.IvanMurzak.Unity.MCP.Common;
using com.IvanMurzak.Unity.MCP.Utils;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace com.IvanMurzak.Unity.MCP.Editor.Tests
{
    public partial class TestSerializer
    {
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            Debug.Log($"[{nameof(TestToolGameObject)}] SetUp");
            yield return null;
        }
        [UnityTearDown]
        public IEnumerator TearDown()
        {
            Debug.Log($"[{nameof(TestToolGameObject)}] TearDown");
            yield return null;
        }

        [UnityTest]
        public IEnumerator SerializeMaterial()
        {
            var material = new Material(Shader.Find("Standard"));

            var serialized = Serializer.Serialize(material);
            var json = JsonUtils.Serialize(serialized);
            Debug.Log($"[{nameof(TestSerializer)}] Result:\n{json}");

            var glossinessValue = 1.0f;
            var colorValue = new Color(1.0f, 0.0f, 0.0f, 0.5f);

            serialized.SetPropertyValue("_Glossiness", glossinessValue);
            serialized.SetPropertyValue("_Color", colorValue);

            var objMaterial = (object)material;
            Serializer.Populate(ref objMaterial, serialized);

            Assert.AreEqual(glossinessValue, material.GetFloat("_Glossiness"), 0.001f, $"Material property '_Glossiness' should be {glossinessValue}.");
            Assert.AreEqual(colorValue, material.GetColor("_Color"), $"Material property '_Glossiness' should be {glossinessValue}.");

            yield return null;
        }
    }
}