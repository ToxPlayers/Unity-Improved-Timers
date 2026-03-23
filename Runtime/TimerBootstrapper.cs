using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace TickTimers { 
    internal static class TimerBootstrapper {
        static PlayerLoopSystem _timerSystem;
        static PlayerLoopSystem _fixedTimerSystem;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        internal static void Initialize() {
            PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();

            if (!InsertTimerManager<Update>(ref _timerSystem, ref currentPlayerLoop, TimerManager.UpdateTimers, 0)) {
                Debug.LogWarning("Tick Timers not initialized, unable to register TimerManager into the Update loop.");
                return;
            }

            if (!InsertTimerManager<FixedUpdate>(ref _fixedTimerSystem, ref currentPlayerLoop, TimerManager.UpdateTimers, 1)) {
                Debug.LogWarning("Tick Timers not initialized, unable to register TimerManager into the Update loop.");
                return;
            }

            PlayerLoop.SetPlayerLoop(currentPlayerLoop);
            
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeState;
            EditorApplication.playModeStateChanged += OnPlayModeState;
            
            static void OnPlayModeState(PlayModeStateChange state) {
                if (state == PlayModeStateChange.ExitingPlayMode)
                {
                    PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
                    RemoveTimerManager(ref currentPlayerLoop); 
                    EditorApplication.delayCall += TimerManager.DisposeOnPlayModeExit;
                }
            }
#endif
        }

        static void RemoveTimerManager(ref PlayerLoopSystem loop) {
            PlayerLoopUtils.RemoveSystem<Update>(ref loop, in _timerSystem);
            PlayerLoopUtils.RemoveSystem<FixedUpdate>(ref loop, in _fixedTimerSystem);
        }

        static bool InsertTimerManager<T>(ref PlayerLoopSystem toInsert, ref PlayerLoopSystem loop, PlayerLoopSystem.UpdateFunction actionCall, int index) {
            toInsert = new PlayerLoopSystem() {
                type = typeof(TimerManager),
                updateDelegate = actionCall,
                subSystemList = null
            };
            return PlayerLoopUtils.InsertSystem<T>(ref loop, in toInsert, index);
        }
    }
}
