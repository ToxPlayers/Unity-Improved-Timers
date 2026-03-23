using System;
using UnityEngine;
using static Unity.IntegerTime.RationalTime;

namespace TickTimers {
    /// <summary>
    /// Timer that ticks at a specific frequency. (<see cref="TicksPerTime"/> times per <see cref="PerSeconds"/> seconds)
    /// DOESNT INCLUDE TICKING TWICE AT ONCE FOR MORE THAN ONE TICK A FRAME!!!
    /// </summary>
    [Serializable]
    public class FrequencyTimer : TickTimerBase {
        public int TicksPerTime = 10;
        public float PerSeconds = 1f;
        public Action OnTickEvent = delegate { };
        
        float timeThreshold;
        public FrequencyTimer() : base() { CalculateTimeThreshold(10); }

        public FrequencyTimer(int ticksPerSecond) : base() {
            CalculateTimeThreshold(ticksPerSecond);
        }

        protected override void OnTick() {
            if (IsTicking && TimeTicked >= timeThreshold) {
                TimeTicked -= timeThreshold;
                OnTickEvent.Invoke();
            }

            if (IsTicking && TimeTicked < timeThreshold) {
                TimeTicked += GetDeltaTime();
            }
        }

        public override bool IsTimerOver => !IsTicking;

        public override void ResetTime() {
            TimeTicked = 0;
        }
        
        public void Reset(int newTicksPerSecond) {
            CalculateTimeThreshold(newTicksPerSecond);
            ResetTime();
        }
        
        void CalculateTimeThreshold(int ticksPerSecond) {
            TicksPerTime = ticksPerSecond;
            timeThreshold = PerSeconds / TicksPerTime;
        }

        protected override void Dispose(bool disposing) {
            if (IsDisposed) return;
            if (disposing)
                OnTickEvent = null;
            base.Dispose(disposing);
        }

        public override string ToString()
        {
            return $"FreqTimer({TicksPerTime:F2}/{TicksPerTime:F2}s over {TimeTicked:F2}s)"; 
        }
         
    }
}