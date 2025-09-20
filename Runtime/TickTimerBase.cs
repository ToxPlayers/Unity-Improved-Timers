using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace ScaledTimers {
     
    [Serializable]
    public abstract class TickTimerBase : IDisposable {
        [ShowInInspector, ReadOnly, HideInEditorMode] public float TimeTicked { get; protected set; }
        [ShowInInspector, ReadOnly, HideInEditorMode] public bool IsTicking { get; private set; }
        [ShowInInspector, HideInEditorMode] public abstract bool IsTimerOver { get; }
        public bool UseUnscaledTime = false;
        public event Action OnTimerStart = delegate { };
        public event Action OnTimerStop = delegate { };
        public event Action<bool> OnIsTimerTicking = delegate { };
        public bool IsRegistered { get; private set; }
        public float GetDeltaTime()
        {
            return UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        } 

        protected TickTimerBase() { }

        /// <summary> 
        /// Resets and registers the timer<br/>
        /// Invokes <see cref="OnTimerStop"/> and <see cref="OnIsTimerTicking"/>
        /// </summary>
        [HorizontalGroup("buttons"), Button, HideIf(nameof(IsTicking)), HideInEditorMode]
        public void Restart() {
            Reset();
            if (!IsTicking) {
                SetIsTicking(true);
                if (!IsRegistered)
                {
                    IsRegistered = true;
                    TimerManager.RegisterTimer(this);
                }
                OnTimerStart.Invoke();
            }
        }

        /// <summary> 
        /// Stops the deregisters the timer<br/>
        /// Invokes <see cref="OnTimerStop"/> and <see cref="OnIsTimerTicking"/>
        /// </summary>
        [HorizontalGroup("buttons"), Button, ShowIf(nameof(IsTicking)), HideInEditorMode]
        public void Stop() {
            if (IsTicking) {
                SetIsTicking(false);
                if(IsRegistered)
                {
                    IsRegistered = false;
                    TimerManager.DeregisterTimer(this);
                }
                OnTimerStop.Invoke();
            }
        }
        public abstract void Tick();

        /// <summary> 
        /// Doesn't start or resets the timer, just sets if its paused or running
        /// Invokes <see cref="OnIsTimerTicking"/>
        /// </summary>
        public void SetIsTicking(bool isRunning)
        {
            IsTicking = isRunning;
            OnIsTimerTicking.Invoke(IsTicking);
        }
        /// <summary>
        /// Toggles between <see cref="Pause"/> and <see cref="Resume"/>
        /// </summary>
        public void ToggleIsTicking() => SetIsTicking(!IsTicking);
        /// <summary>
        /// same as <see cref="SetIsTicking"/> true
        /// </summary>
        [HorizontalGroup("buttons"), Button, HideIf(nameof(IsTicking)), HideInEditorMode]
        public void Resume() => SetIsTicking(true);

        /// <summary>
        /// same as <see cref="SetIsTicking"/> false
        /// </summary>
        [HorizontalGroup("buttons"), Button, ShowIf(nameof(IsTicking)), HideInEditorMode]
        public void Pause() => SetIsTicking(false);
        public virtual void Reset() => TimeTicked = 0;


        bool _disposed;

        ~TickTimerBase() {
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