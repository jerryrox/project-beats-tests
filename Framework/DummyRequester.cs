using System;
using System.Collections;
using System.Collections.Generic;

namespace PBFramework.Tests
{
    /// <summary>
    /// A dummy class used for testing with an IPromise implementation.
    /// </summary>
    public class DummyRequester<T> : IPromise<T> {

        public event Action<T> OnFinishedResult;
        public event Action OnFinished
        {
            add => OnFinishedResult += (v) => value();
            remove => OnFinishedResult -= (v) => value();
        }

        public event Action<float> OnProgress;

        public T Result { get; set; }
        object IPromise.Result
        {
            get => Result;
        }

        public bool IsFinished { get; set; }

        public float Progress { get; private set; }

        public virtual void Start()
        {
            IsFinished = false;
        }

        public virtual void Revoke()
        {
            IsFinished = false;
        }

        public virtual void DoFinish(T value)
        {
            Result = value;
            OnFinishedResult(value);
        }

        public virtual void SetProgress(float progress)
        {
            Progress = progress;
            OnProgress?.Invoke(progress);
        }
    }
}