using System.Collections.Generic;
using UnityEngine;

namespace TickTimers {
    public static class TimerManager {
        static readonly HashSet<TickTimerBase> timers = new();
        static readonly List<TickTimerBase> sweep = new();
        
        public static void RegisterTimer(TickTimerBase timer) => timers.Add(timer);
        public static void DeregisterTimer(TickTimerBase timer) => timers.Remove(timer);

        public static void UpdateTimers() {
            if (timers.Count == 0) 
                return;

            sweep.Clear();
            sweep.AddRange(timers);
            foreach (var timer in sweep) {
                timer.Tick();
            }
        }
        
        public static void DisposeOnPlayModeExit() {
            if(timers.Count > 0)
                Debug.LogWarning($"TimerManager: {timers.Count} Timers were not disposed");

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