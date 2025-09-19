using System;
using UnityEngine;

namespace ScaledTimers {
    /// <summary>
    /// Timer that ticks at a specific frequency. (N times per P seconds)
    /// </summary>
    [Serializable]
    public class FrequencyTimer : ScaledTimerBase {
        public int TicksPerTime { get; private set; }
        public float PerSeconds = 1f;
        public Action OnTick = delegate { };
        
        float timeThreshold;

        public FrequencyTimer(int ticksPerSecond) : base() {
            CalculateTimeThreshold(ticksPerSecond);
        }

        public override void Tick() {
            if (IsRunning && TimeRunning >= timeThreshold) {
                TimeRunning -= timeThreshold;
                OnTick.Invoke();
            }

            if (IsRunning && TimeRunning < timeThreshold) {
                TimeRunning += Time.deltaTime;
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
    }
}