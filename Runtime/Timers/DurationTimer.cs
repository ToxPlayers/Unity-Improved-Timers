using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace TickTimers {
    /// <summary>
    /// Timer for duration of <see cref="MaxTime"/>
    /// </summary> 
    [Serializable]
    public class DurationTimer : TickTimerBase {

        [PropertyOrder(-1)]
        public float MaxTime;
        public bool StopTickingOnTimerOver;
        public bool DeregisterOnTimerOver;
        public float NormalizedTimeUnclamped
        {
            get
            {
                if (MaxTime == 0)
                    return 1f;
                return TimeTicked / MaxTime;
            }
        }
        [ProgressBar(0, 1), ShowIf(nameof(IsTicking) + "|| Application.isPlaying")]
        public float NormalizedTime => Mathf.Clamp01(NormalizedTimeUnclamped);
        public float Countdown => NormalizedTime - 1f;
        public override bool IsTimerOver => ReachedTime(MaxTime); 
        public int LoopsCount => Mathf.FloorToInt(NormalizedTimeUnclamped);
        public float DurationOvershot
        {
            get
            {
                if(TimeTicked < MaxTime)
                    return 0f;
                var normTime = NormalizedTimeUnclamped;
                var flooredNormTime = Mathf.Floor(normTime);
                return normTime - flooredNormTime;
            }
        } 

        public DurationTimer() : base() { MaxTime = 0f; }
        public DurationTimer(float maxTime) : base() { MaxTime = maxTime; }
        public event Action OnTimerOver = delegate { };
        int _lastLoopCountInvoked = 0; 

        protected override void OnTick() {
            if (IsTicking)
            {
                if (MaxTime > 0)
                    TimeTicked += GetDeltaTime();

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

                if(isTimerOver)
                {
                    if (DeregisterOnTimerOver)
                        StopAndDeregister();
                    else if (StopTickingOnTimerOver)
                        SetIsTicking(false);
                }
            }
        }

        public void ResetWithOvershotOffset()
        {
            var offset = DurationOvershot;
            ResetTime();
            TimeTicked += offset;
        }

        public override void ResetTime()
        {
            base.ResetTime();
            _lastLoopCountInvoked = 0;
        } 

        public void SetTimerOver() {
            TimeTicked = MaxTime;
        }

        public override string ToString()
        {
            return $"Duration({TimeTicked:F2} / {MaxTime:F2} = {NormalizedTimeUnclamped:F2})";
        }
    }
}