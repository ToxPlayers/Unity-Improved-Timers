using System;
using UnityEngine;

namespace TickTimers {
    /// <summary>
    /// Timer that counts up from zero to infinity. 
    /// </summary>
    [Serializable]
    public class StopwatchTimer : TickTimerBase {
        public StopwatchTimer() : base() { }
        protected override void OnTick() {
            if (IsTicking) {
                TimeTicked += GetDeltaTime();
            }
        }
        public override bool IsTimerOver => false;
        public override string ToString()
        {
            return "Stopwatch(" + TimeTicked + ")";
        }

    }
}