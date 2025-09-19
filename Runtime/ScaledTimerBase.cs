using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace ScaledTimers {
    [Serializable]
    public abstract class ScaledTimerBase : IDisposable {
        [ShowInInspector, ReadOnly] public float TimeRunning { get; protected set; }
        [ShowInInspector, ReadOnly] public bool IsRunning { get; private set; }

        public event Action OnTimerStart = delegate { };
        public event Action OnTimerStop = delegate { };
        public event Action<bool> OnIsTimerRunning = delegate { };
        protected ScaledTimerBase() { }
         
        public void Start() {
            TimeRunning = 0;
            if (!IsRunning) {
                SetIsTimerRunning(true);
                TimerManager.RegisterTimer(this);
                OnTimerStart.Invoke();
            }
        }

        public void Stop() {
            if (IsRunning) {
                SetIsTimerRunning(false);
                TimerManager.DeregisterTimer(this);
                OnTimerStop.Invoke();
            }
        }
        public abstract void Tick();
        public abstract bool IsTimerOver { get; }
        public void SetIsTimerRunning(bool isRunning)
        {
            IsRunning = isRunning;
            OnIsTimerRunning.Invoke(IsRunning);
        }
        public void Resume() => SetIsTimerRunning(true);  
        public void Pause() => SetIsTimerRunning(false);
        public virtual void Reset() => TimeRunning = 0;

        public virtual void Reset(float newTime) {
            Reset();
        }

        bool _disposed;

        ~ScaledTimerBase() {
            Dispose(false);
        }

        // Call Dispose to ensure deregistration of the timer from the TimerManager
        // when the consumer is done with the timer or being destroyed
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (_disposed) return;

            if (disposing) {
                TimerManager.DeregisterTimer(this);
            }

            _disposed = true;
        }
    }
}