using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PBFramework.Threading;

namespace PBFramework.Networking.Tests
{
    public class AudioRequestTest {
        
        [UnityTest]
        public IEnumerator TestNonStream()
        {
            var request = new AudioRequest(TestConstants.RemoteMp3Url, false);
            var listener = new TaskListener<IWebRequest>();
            request.Request(listener);

            while (!request.IsFinished)
            {
                Debug.Log("Progress: " + listener.Progress);
                yield return null;
            }

            Assert.IsNotNull(request.Response);

            var clip = request.Response.AudioData;
            Assert.IsNotNull(clip);

            Debug.Log($"Content: {request.Response.ContentLength}, response: {request.Response.BytesLoaded}");
            Assert.AreEqual(request.Response.ContentLength, request.Response.BytesLoaded);
        }

        [UnityTest]
        public IEnumerator TestStream()
        {
            // Current limitation:
            // There is no way to check whether the loaded audio is truly a streaming audio or not.

            var request = new AudioRequest(TestConstants.RemoteMp3Url, true);
            var listener = new TaskListener<IWebRequest>();
            request.Request(listener);

            while (!request.IsFinished)
            {
                Debug.Log("Progress: " + listener.Progress);
                yield return null;
            }

            Assert.IsNotNull(request.Response);

            var clip = request.Response.AudioData;
            Assert.IsNotNull(clip);

            Debug.Log($"Content: {request.Response.ContentLength}, response: {request.Response.BytesLoaded}");
            Assert.LessOrEqual((double)request.Response.BytesLoaded, (double)request.Response.ContentLength);
        }

        [UnityTest]
        public IEnumerator TestTask()
        {
            var request = new AudioRequest(TestConstants.RemoteMp3Url, false);
            ITask<AudioClip> task = request;
            Assert.AreEqual(request, task);
            Assert.IsFalse(task.DidRun);
            Assert.IsFalse(task.IsFinished);

            // Receive via callback
            AudioClip clip = null;
            TaskListener<AudioClip> listener = new TaskListener<AudioClip>();
            listener.OnFinished += (value) => clip = value;

            // Request
            task.StartTask(listener);
            Assert.IsFalse(task.IsFinished);
            Assert.IsFalse(request.IsFinished);

            // Wait till finish
            while (!task.IsFinished)
            {
                Debug.Log("Progress: " + request.Progress);
                yield return null;
            }

            Assert.IsTrue(task.DidRun);
            Assert.IsTrue(task.IsFinished);
            Assert.IsTrue(request.IsFinished);
            Assert.IsNotNull(request.Response);
            Assert.IsNotNull(clip);
            Assert.AreEqual(clip, request.Response.AudioData);
            Assert.AreEqual(listener.Value, clip);
        }
    }
}