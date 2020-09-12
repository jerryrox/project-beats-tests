using PBFramework.Threading;

namespace PBFramework.Allocation.Caching.Tests
{
    public class DummyCacher : Cacher<string, DummyCacherData> {

        protected override ITask<DummyCacherData> CreateRequest(string key)
        {
            return new ManualTask<DummyCacherData>((f) => RunDummyTask(f, key));
        }

        protected override void DestroyData(DummyCacherData data)
        {
            data.IsDestroyed = true;
        }

        private void RunDummyTask(ManualTask<DummyCacherData> task, string key)
        {
            var timer = new SynchronizedTimer()
            {
                Limit = 1f
            };
            timer.OnFinished += () =>
            {
                task.SetFinished(new DummyCacherData()
                {
                    Key = key,
                    IsDestroyed = false
                });
            };
            timer.OnProgress += task.SetProgress;
            timer.Start();
        }
    }
}