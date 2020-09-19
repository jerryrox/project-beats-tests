using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Testing.Tests
{
    public class TestExtensionsTest {
        
        [Test]
        public void TestFindFromPath()
        {
            var agent = Setup();
            Assert.IsNotNull(agent.FindFromPath<BoxCollider>("collider"));
            Assert.IsNotNull(agent.FindFromPath<Rigidbody>("rigidbody"));
            Assert.IsNotNull(agent.FindFromPath<AudioSource>("rigidbody/audio"));
            Assert.IsNull(agent.FindFromPath<AudioSource>("audio"));
        }

        [Test]
        public void TestFindWithName()
        {
            var agent = Setup();
            Assert.IsNotNull(agent.FindWithName<BoxCollider>("collider"));
            Assert.IsNotNull(agent.FindWithName<Rigidbody>("rigidbody"));
            Assert.IsNotNull(agent.FindWithName<Rigidbody>("rigidbody"));
            Assert.IsNotNull(agent.FindWithName<AudioSource>("audio"));
            Assert.IsNull(agent.FindWithName<AudioSource>("rigidbody/audio"));
        }

        private DummyMonobehaviour Setup()
        {
            var oldAgent = GameObject.Find("_TestExtensionsTestAgent");
            if(oldAgent != null)
                GameObject.Destroy(oldAgent);

            GameObject agent = new GameObject("_TestExtensionsTestAgent");
            {
                BoxCollider collider = new GameObject("collider").AddComponent<BoxCollider>();
                collider.transform.SetParent(agent.transform);

                Rigidbody rigidBody = new GameObject("rigidbody").AddComponent<Rigidbody>();
                rigidBody.transform.SetParent(agent.transform);
                {
                    AudioSource audioSource = new GameObject("audio").AddComponent<AudioSource>();
                    audioSource.transform.SetParent(rigidBody.transform);
                }
            }
            return agent.AddComponent<DummyMonobehaviour>();
        }
    }
}