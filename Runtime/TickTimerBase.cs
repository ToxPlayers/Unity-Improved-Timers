using Sirenix.OdinInspector;
using System;
using System.Runtime.CompilerServices;
using TickTimers;
using UnityEngine;
using Conditional = System.Diagnostics.ConditionalAttribute;

namespace TickTimers {
     
    [Serializable, LabelText(SdfIconType.Clock)]
    public abstract class TickTimerBase : IDisposable {
#if UNITY_EDITOR
        public string TIMER_SOURCE_DEBUG = "Unknown";
#endif

        [Conditional("UNITY_EDITOR")]
        public void SetTimerSourceDebug(in string src)
#if UNITY_EDITOR 
            { TIMER_SOURCE_DEBUG = src; }
#else
            {}
#endif
        [ShowInInspector, ReadOnly, HideInEditorMode] public float TimeTicked { get; protected set; }
        [ShowInInspector, ReadOnly, HideInEditorMode] public bool IsTicking { get; private set; }
        [ShowInInspector, HideInEditorMode] public abstract bool IsTimerOver { get; }
        public bool UseUnscaledTime = false;
        public bool FixedUpdateMode = false;
        public event Action<bool> OnIsTimerTicking = delegate { };
        public bool ReachedTime(float maxTime) => TimeTicked >= maxTime;
        public bool IsRegistered { get; private set; }

        public float GetDeltaTime()
        { 
            if (Time.inFixedTimeStep)
                return UseUnscaledTime ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime;
            return UseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        } 
        protected TickTimerBase() { } 
        /// <summary> 
        /// Resets and registers the timer<br/>
        /// Invokes <see cref="OnTimerStop"/> and <see cref="OnIsTimerTicking"/>
        /// </summary>
        [HorizontalGroup("buttons"), Button, HideIf(nameof(IsTicking)), HideInEditorMode] 
        public void Restart([CallerMemberName] string src = null) { 
            if (IsDisposed)
            {
                Debug.LogError("Tried restarting a disposed timer");
                return;
            }
            ResetTime(); 
            SetIsTicking(true);  
        }

        /// <summary> 
        /// Stops the deregisters the timer<br/>
        /// Invokes <see cref="OnTimerStop"/> and <see cref="OnIsTimerTicking"/>
        /// </summary>
        [HorizontalGroup("buttons"), Button, ShowIf(nameof(IsTicking)), HideInEditorMode]
        public void StopAndDeregister() {
            if (IsDisposed) {
                Debug.LogError("tried deregister a disposed timer");
                return;
            }

            SetIsTicking(false);

            if (IsRegistered) {
                IsRegistered = false;
                TimerManager.DeregisterTimer(this);
            }
        }

        internal virtual void Tick() {
#if UNITY_EDITOR
            if (Time.inFixedTimeStep != FixedUpdateMode) {
                Debug.LogError("Tried ticking timer outside of the correct update mode");
                return;
            }
#endif
            OnTick(); 
        }

        protected abstract void OnTick();

        
        /// <summary> 
        /// Doesn't start or resets the timer, just sets if its paused or running
        /// Invokes <see cref="OnIsTimerTicking"/>
        /// </summary>
        public void SetIsTicking(bool isRunning)
        {
            if (IsTicking == isRunning)
                return;

            IsTicking = isRunning;
            OnIsTimerTicking.Invoke(IsTicking);

            if(IsTicking && !IsRegistered) {
                IsRegistered = true;
                TimerManager.RegisterTimer(this);
            }
        }
        /// <summary>
        /// Toggles between <see cref="Pause"/> and <see cref="Resume"/>
        /// </summary>
        public void ToggleIsTicking() => SetIsTicking(!IsTicking);
        /// <summary> 
        /// Resumes the timer. Same as <seealso cref="SetIsTicking(bool)"/> set to true
        /// <br/>Invokes <seealso cref="OnIsTimerTicking"/>
        /// </summary>
        [HorizontalGroup("buttons"), Button, HideIf(nameof(IsTicking)), HideInEditorMode]
        public void Resume() => SetIsTicking(true);

        /// <summary> 
        /// Pauses the timer. Same as <seealso cref="SetIsTicking(bool)"/> set to false
        /// <br/>Invokes <seealso cref="OnIsTimerTicking"/>
        /// </summary>
        [HorizontalGroup("buttons"), Button, ShowIf(nameof(IsTicking)), HideInEditorMode]
        public void Pause() => SetIsTicking(false);
        public virtual void ResetTime() => TimeTicked = 0;


        [ShowInInspector, ShowIf(nameof(IsDisposed)), InfoBox("Timer is disposed", InfoMessageType = InfoMessageType.Warning)]
        public bool IsDisposed { get; private set; }

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
            if (IsDisposed) return;

            if (disposing) { 
                StopAndDeregister(); 
            }

            IsDisposed = true;
        }
    }
}