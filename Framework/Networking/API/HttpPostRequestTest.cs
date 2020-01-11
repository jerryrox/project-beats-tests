using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Networking.API.Tests
{
    public class HttpPostRequestTest {
        
        [UnityTest]
        public IEnumerator TestPlain()
        {
            var request = new HttpPostRequest("http://httpbin.org/post");
            request.Request();

            while(!request.IsFinished) yield return null;

            Debug.Log($"TestPlain result:\n{request.Response.TextData}");
        }

        [UnityTest]
        public IEnumerator TestBinary()
        {
            var request = new HttpPostRequest("http://httpbin.org/post");

            var data = new BinaryPostData(Encoding.UTF8.GetBytes("Lolz"));
            request.SetPostData(data);
            request.Request();

            while(!request.IsFinished) yield return null;

            Debug.Log($"TestBinary result:\n{request.Response.TextData}");
        }

        [UnityTest]
        public IEnumerator TestForm()
        {
            var request = new HttpPostRequest("http://httpbin.org/post");

            var data = new FormPostData();
            data.AddBinary("binary", Encoding.UTF8.GetBytes("troll"));
            data.AddField("a1", "a111");
            data.AddField("a2", 22);
            data.AddFile("filez", Encoding.UTF8.GetBytes("content"), "myFile.txt");
            request.SetPostData(data);
            request.Request();

            while(!request.IsFinished) yield return null;

            Debug.Log($"TestForm result:\n{request.Response.TextData}");
        }

        [UnityTest]
        public IEnumerator TestRawText()
        {
            var request = new HttpPostRequest("http://httpbin.org/post");

            var data = new RawPostData("raw-text", RawPostData.Types.Text);
            request.SetPostData(data);
            request.Request();

            while(!request.IsFinished) yield return null;

            Debug.Log($"TestRawText result:\n{request.Response.TextData}");
        }

        [UnityTest]
        public IEnumerator TestRawJson()
        {
            var request = new HttpPostRequest("http://httpbin.org/post");

            var data = new RawPostData("{\"a\":\"b\"}", RawPostData.Types.Json);
            request.SetPostData(data);
            request.Request();

            while(!request.IsFinished) yield return null;

            Debug.Log($"TestRawJson result:\n{request.Response.TextData}");
        }

        [UnityTest]
        public IEnumerator TestRawJavascript()
        {
            var request = new HttpPostRequest("http://httpbin.org/post");

            var data = new RawPostData("console.log('asdf');", RawPostData.Types.Javascript);
            request.SetPostData(data);
            request.Request();

            while(!request.IsFinished) yield return null;

            Debug.Log($"TestRawJavascript result:\n{request.Response.TextData}");
        }

        [UnityTest]
        public IEnumerator TestRawHtml()
        {
            var request = new HttpPostRequest("http://httpbin.org/post");

            var data = new RawPostData("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 3.2 Final//EN\"><title>405 Method Not Allowed</title><h1>Method Not Allowed</h1><p>The method is not allowed for the requested URL.</p>", RawPostData.Types.Html);
            request.SetPostData(data);
            request.Request();

            while(!request.IsFinished) yield return null;

            Debug.Log($"TestRawHtml result:\n{request.Response.TextData}");
        }
        
        [UnityTest]
        public IEnumerator TestRawXml()
        {
            var request = new HttpPostRequest("http://httpbin.org/post");

            var data = new RawPostData("<my data=\"troll\"/>", RawPostData.Types.Xml);
            request.SetPostData(data);
            request.Request();

            while(!request.IsFinished) yield return null;

            Debug.Log($"TestRawXml result:\n{request.Response.TextData}");
        }
    }
}