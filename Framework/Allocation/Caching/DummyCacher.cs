using PBFramework.Threading;
using PBFramework.Threading.Futures;

namespace PBFramework.Allocation.Caching.Tests
{
    public class DummyCacher : Cacher<string, DummyCacherData> {

        protected override IControlledFuture<DummyCacherData> CreateRequest(string key)
        {
            return new Future<DummyCacherData>((f) => RunDummyTask(f, key));
        }

        protected override void DestroyData(DummyCacherData data)
        {
            data.IsDestroyed = true;
        }

        private void RunDummyTask(Future<DummyCacherData> future, string key)
        {
            var timer = new SynchronizedTimer()
            {
                Limit = 1f
            };
            timer.IsCompleted.OnNewValue += (completed) =>
            {
                if(completed)
                    future.SetComplete(new DummyCacherData()
                    {
                        Key = key,
                        IsDestroyed = false
                });
            };
            timer.Progress.OnNewValue += future.SetProgress;
            timer.Start();
        }
    }
}