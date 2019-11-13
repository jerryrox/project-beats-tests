using System;
using System.Collections;
using System.Collections.Generic;
using PBFramework.Tests;
using PBFramework.Threading;

namespace PBFramework.Allocation.Caching.Tests
{
    public class DummyCacherRequester : DummyRequester<DummyCacherData>
    {
        private string key;
        private SynchronizedTimer timer;


        public DummyCacherRequester(string key)
        {
            this.key = key;
        }

        public override void Start()
        {
            if(timer != null) return;

            base.Start();

            timer = new SynchronizedTimer()
            {
                Limit = 1f
            };
            timer.OnProgress += SetProgress;
            timer.OnFinished += delegate
            {
                FinishRequest();
            };
            timer.Start();
        }

        public override void Revoke()
        {
            if(timer == null) return;

            base.Revoke();
            timer.Stop();
            timer = null;
        }

        private void FinishRequest()
        {
            DoFinish(new DummyCacherData() {
                Key = key,
                IsDestroyed = false
            });
            timer = null;
        }
    }
}