using System;
using UnityEngine;
using static Unity.IntegerTime.RationalTime;

namespace ScaledTimers {
    /// <summary>
    /// Timer that ticks at a specific frequency. (<see cref="TicksPerTime"/> times per <see cref="PerSeconds"/> seconds)
    /// DOESNT INCLUDE TICKING TWICE AT ONCE FOR MORE THAN ONE TICK A FRAME!!!
    /// </summary>
    [Serializable]
    public class FrequencyTimer : ScaledTimerBase {
        public int TicksPerTime = 10;
        public float PerSeconds = 1f;
        public Action OnTick = delegate { };
        
        float timeThreshold;
        public FrequencyTimer() : base() { CalculateTimeThreshold(10); }

        public FrequencyTimer(int ticksPerSecond) : base() {
            CalculateTimeThreshold(ticksPerSecond);
        }

        public override void Tick() {
            if (IsRunning && TimeRunning >= timeThreshold) {
                TimeRunning -= timeThreshold;
                OnTick.Invoke();
            }

            if (IsRunning && TimeRunning < timeThreshold) {
                TimeRunning += GetDeltaTime();
            }
        }

        public override bool IsTimerOver => !IsRunning;

        public override void Reset() {
            TimeRunning = 0;
        }
        
        public void Reset(int newTicksPerSecond) {
            CalculateTimeThreshold(newTicksPerSecond);
            Reset();
        }
        
        void CalculateTimeThreshold(int ticksPerSecond) {
            TicksPerTime = ticksPerSecond;
            timeThreshold = PerSeconds / TicksPerTime;
        }

        public override string ToString()
        {
            return $"FreqTimer({TicksPerTime:F2}/{TicksPerTime:F2}s over {TimeRunning:F2}s)"; 
        }
    }
}