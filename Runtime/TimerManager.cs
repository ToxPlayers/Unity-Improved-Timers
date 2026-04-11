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
        public static void UpdateTimers() {
            if (_timers.Count == 0) 
                return;

            IsUpdatingTimers = true;

            _timers.UnionWith(_toRegister);
            _timers.ExceptWith(_toUnregister);
            var isFixedStep = Time.inFixedTimeStep;
            foreach (var timer in _timers)
                if(timer.FixedUpdateMode == isFixedStep)
                    timer.Tick(); 

            IsUpdatingTimers = false;
        }
        
        public static void DisposeOnPlayModeExit() 
        { 
            if(_timers.Count > 0)
            {
                string timersNotDisposedLog = $"TimerManager: {_timers.Count} Timers were not disposed:";
                foreach (var timer in _timers) {
                    timersNotDisposedLog += '\n' + timer.ToString();
#if UNITY_EDITOR
                    timersNotDisposedLog += $"from [{timer.TIMER_SOURCE}";
#endif
                }
                Debug.Log(timersNotDisposedLog);
            }
            var copySet = new HashSet<TickTimerBase>(_timers);
            foreach (var timer in copySet) 
                timer.Dispose();
            _timers.Clear(); 
        }
    }
}