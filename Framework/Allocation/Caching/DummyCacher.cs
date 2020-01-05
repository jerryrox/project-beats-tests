using System;
using System.Collections;
using System.Collections.Generic;

namespace PBFramework.Allocation.Caching.Tests
{
    public class DummyCacher : Cacher<string, DummyCacherData> {

        protected override IPromise<DummyCacherData> CreateRequest(string key) => new DummyCacherRequester(key);

        protected override void DestroyData(DummyCacherData data)
        {
            data.IsDestroyed = true;
        }
    }
}