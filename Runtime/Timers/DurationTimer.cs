using System;
using UnityEngine;

namespace ScaledTimers {
    /// <summary>
    /// Timer for duration of <see cref="MaxTime"/>
    /// </summary> 
    [Serializable]
    public class DurationTimer : ScaledTimerBase {
         
        public float MaxTime;
        public bool StopOnTimerOver;
        public float NormalizedTimeUnclamped
        {
            get
            {
                if (MaxTime == 0)
                    return 1f;
                return TimeRunning / MaxTime;
            }
        }
        public float NormalizedTime => Mathf.Clamp01(NormalizedTimeUnclamped);
        public float Countdown => NormalizedTime - 1f;
        public override bool IsTimerOver => TimeRunning >= MaxTime;
        public int LoopsCount => Mathf.FloorToInt(NormalizedTimeUnclamped);
        public DurationTimer() : base() { MaxTime = 0f; }
        public DurationTimer(float maxTime) : base() { MaxTime = maxTime; }
        public event Action OnTimerOver = delegate { };
        int _lastLoopCountInvoked = 0;

        public virtual void Reset(float maxTime)
        {
            MaxTime = maxTime;
            Reset();
        }
        public override void Tick() {
            if (IsRunning)
            {
                if (MaxTime > 0)
                    TimeRunning += GetDeltaTime();

                var isTimerOver = IsTimerOver;
                var loopCount = LoopsCount;
                if (loopCount != _lastLoopCountInvoked)
                {
                    if(isTimerOver)
                    {
                        _lastLoopCountInvoked = Mathf.Min(_lastLoopCountInvoked, loopCount - 1);
                        for (int i = _lastLoopCountInvoked; i < loopCount; i++) 
                            OnTimerOver.Invoke();
                    }
                    _lastLoopCountInvoked = loopCount;
                }

                if (StopOnTimerOver && isTimerOver)
                    Stop();
            }
        }

        public override void Reset()
        {
            base.Reset();
            _lastLoopCountInvoked = 0;
        }

        public override string ToString()
        {
            return $"Duration({TimeRunning:F2} / {MaxTime:F2} = {NormalizedTimeUnclamped:F2})";
        }
    }
}