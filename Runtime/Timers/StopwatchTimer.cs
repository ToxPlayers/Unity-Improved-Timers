using System;
using UnityEngine;

namespace TickTimers {
    /// <summary>
    /// Timer that counts up from zero to infinity. 
    /// </summary>
    [Serializable]
    public class StopwatchTimer : TickTimerBase {
        public StopwatchTimer() : base() { }
        internal override void Tick() {
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