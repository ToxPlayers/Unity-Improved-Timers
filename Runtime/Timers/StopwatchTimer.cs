using System;
using UnityEngine;

namespace ScaledTimers {
    /// <summary>
    /// Timer that counts up from zero to infinity. 
    /// </summary>
    [Serializable]
    public class StopwatchTimer : ScaledTimerBase {
        public StopwatchTimer() : base() { }
        public override void Tick() {
            if (IsRunning) {
                TimeRunning += GetDeltaTime();
            }
        }
        public override bool IsTimerOver => false;
        public override string ToString()
        {
            return "Stopwatch(" + TimeRunning + ")";
        }
    }
}