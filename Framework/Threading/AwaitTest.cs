using System;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Threading
{
    public class AwaitTest {
        
        public async void Whatever()
        {
            DummyAwaitable dummy = new DummyAwaitable();
            await dummy;
        }

        private class DummyAwaitable
        {
            public CustomAwaiter GetAwaiter()
            {
                return null;
            }
        }

        private class CustomAwaiter : INotifyCompletion
        {
            public bool IsCompleted
            {
                get; set;
            }


            public void GetResult()
            {

            }

            void INotifyCompletion.OnCompleted(Action continuation)
            {
            }
        }
    }
}