using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PBFramework.Data.Bindables.Tests
{
    public class ProxyBindableTest : BindableTest {

        protected override IBindable<Dummy> CreateBindable(Dummy dummy)
        {
            Dummy myDummy = dummy;
            return new ProxyBindable<Dummy>(
                () => myDummy,
                (value) => myDummy = value
            );
        }
    }
}