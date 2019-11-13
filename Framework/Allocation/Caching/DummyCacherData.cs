using System;
using System.Collections;
using System.Collections.Generic;

namespace PBFramework.Allocation.Caching.Tests
{
    public class DummyCacherData {

        public string Key { get; set; } = null;

        public bool IsDestroyed { get; set; } = false;
    }
}