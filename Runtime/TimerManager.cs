using System.Collections.Generic;

namespace ScaledTimers {
    public static class TimerManager {
        static readonly HashSet<ScaledTimerBase> timers = new();
        static readonly List<ScaledTimerBase> sweep = new();
        
        public static void RegisterTimer(ScaledTimerBase timer) => timers.Add(timer);
        public static void DeregisterTimer(ScaledTimerBase timer) => timers.Remove(timer);

        public static void UpdateTimers() {
            if (timers.Count == 0) return;

            sweep.Clear();
            sweep.AddRange(timers);
            foreach (var timer in sweep) {
                timer.Tick();
            }
        }
        
        public static void Clear() {
            sweep.Clear();
            sweep.AddRange(timers);
            foreach (var timer in sweep) {
                timer.Dispose();
            }
            
            timers.Clear();
            sweep.Clear();
        }
    }
}