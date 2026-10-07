using System;
using UnityEngine;

namespace TickTimers {
    /// <summary>
    /// Timer that counts up from zero to infinity. 
    /// </summary>
    [Serializable]
    public class StopwatchTimer : TickTimerBase {
        public StopwatchTimer() : base() { }
        public void AddTime(float time) => TimeTicked += time;
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
