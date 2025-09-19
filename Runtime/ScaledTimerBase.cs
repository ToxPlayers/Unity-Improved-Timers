using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace ScaledTimers {
     
    [Serializable]
    public abstract class ScaledTimerBase : IDisposable {
        [ShowInInspector, ReadOnly, HideInEditorMode] public float TimeRunning { get; protected set; }
        [ShowInInspector, ReadOnly, HideInEditorMode] public bool IsRunning { get; private set; }
        [ShowInInspector, HideInEditorMode] public abstract bool IsTimerOver { get; }

        public event Action OnTimerStart = delegate { };
        public event Action OnTimerStop = delegate { };
        public event Action<bool> OnIsTimerRunning = delegate { };
        protected ScaledTimerBase() { }

        [HorizontalGroup("buttons"), Button, HideIf(nameof(IsRunning)), HideInEditorMode]
        public void Restart() {
            Reset();
            if (!IsRunning) {
                SetIsTimerRunning(true);
                TimerManager.RegisterTimer(this);
                OnTimerStart.Invoke();
            }
        }

        [HorizontalGroup("buttons"), Button, ShowIf(nameof(IsRunning)), HideInEditorMode]
        public void Stop() {
            if (IsRunning) {
                SetIsTimerRunning(false);
                TimerManager.DeregisterTimer(this);
                OnTimerStop.Invoke();
            }
        }
        public abstract void Tick();
        public void SetIsTimerRunning(bool isRunning)
        {
            IsRunning = isRunning;
            OnIsTimerRunning.Invoke(IsRunning);
        } 
        public void ToggleIsTimerRunning() => SetIsTimerRunning(!IsRunning);
        [HorizontalGroup("buttons"), Button, HideIf(nameof(IsRunning)), HideInEditorMode]
        public void Resume() => SetIsTimerRunning(true);
        [HorizontalGroup("buttons"), Button, ShowIf(nameof(IsRunning)), HideInEditorMode]
        public void Pause() => SetIsTimerRunning(false);
        public virtual void Reset() => TimeRunning = 0;


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