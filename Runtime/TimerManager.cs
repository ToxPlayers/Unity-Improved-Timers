using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TickTimers {
    public static class TimerManager {
        static readonly HashSet<TickTimerBase> _timers = new();
        static readonly List<TickTimerBase> _toRegister = new();
        static readonly List<TickTimerBase> _toUnregister = new();
        static public bool IsUpdatingTimers { get; private set; }
        public static void RegisterTimer(TickTimerBase timer)
        {
            if (IsUpdatingTimers) {
                _toRegister.Add(timer);
                _toUnregister.Remove(timer);
            }
            else
                _timers.Add(timer);
        }
        public static void DeregisterTimer(TickTimerBase timer)
        {
            if(IsUpdatingTimers) {
                _toRegister.Remove(timer);
                _toUnregister.Add(timer);
            }
            else
                _timers.Remove(timer);
        }

        static void CommitRegistration() { 
            _timers.UnionWith(_toRegister);
            _timers.ExceptWith(_toUnregister);
        }

        public static void UpdateTimers() {
            if (_timers.Count == 0) 
                return;

            IsUpdatingTimers = true;
            CommitRegistration();
            var isFixedStep = Time.inFixedTimeStep;
            foreach (var timer in _timers)
                if(timer.FixedUpdateMode == isFixedStep)
                    timer.Tick(); 

            IsUpdatingTimers = false;
        }
        
        public static void DisposeOnPlayModeExit() 
        {
            CommitRegistration();
            if (_timers.Count > 0)
            {
                string timersNotDisposedLog = $"TimerManager: {_timers.Count} Timers were not disposed:";
                foreach (var timer in _timers) {
                    timersNotDisposedLog += '\n' + timer.GetType().Name;
#if UNITY_EDITOR
                    timersNotDisposedLog += $" -> from [{timer.TIMER_SOURCE_DEBUG}]";
#endif
                }
                Debug.LogWarning(timersNotDisposedLog);
            }
            var copySet = new HashSet<TickTimerBase>(_timers);
            foreach (var timer in copySet) 
                timer.Dispose();
            _timers.Clear(); 
        }
    }
}