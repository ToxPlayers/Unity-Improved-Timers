using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Serialization;
using static Unity.IntegerTime.RationalTime;

namespace TickTimers {
	/// <summary>
	/// Timer that ticks at a specific frequency. (<see cref="TicksPerTime"/> times per <see cref="PerSeconds"/> seconds)
	/// Can tick multiple times per frame if the frame time is longer than the tick frequency.
	/// </summary>
	[Serializable]
    public class FrequencyTimer : TickTimerBase {
        [SerializeField, HideInInspector, FormerlySerializedAs("TicksPerTime")] 
        int _ticksPerTime;
		[SerializeField, HideInInspector, FormerlySerializedAs("PerSeconds")] 
        float _perSeconds;
        [field: SerializeField, ReadOnly]
        public float TimeThreshold { get; private set; } = 0.1f;
		[ShowInInspector]
		public int TicksPerTime {
			get => _ticksPerTime;
			set { _ticksPerTime = value; RecalculateTimeThreshold(); }
		}
		[ShowInInspector]
		public float PerSeconds {
			get => _perSeconds;
			set { _perSeconds = value; RecalculateTimeThreshold(); }
		}
		public Action OnTickEvent = delegate { };

		public FrequencyTimer() : base() { 
            RecalculateTimeThreshold(); 
        }

        public FrequencyTimer(int ticksPerSeconds, float perSeconds = 1f) : base() {
            PerSeconds = perSeconds;
            TicksPerTime = ticksPerSeconds;
            RecalculateTimeThreshold();
        }

        protected override void OnTick() {
            if (!IsTicking) return;
            TimeTicked += GetDeltaTime();
            if (TimeThreshold <= 0f) return;
            while (IsTicking && TimeTicked >= TimeThreshold) {
                TimeTicked -= TimeThreshold;
                OnTickEvent?.Invoke();
            }
        }

        public override bool IsTimerOver => !IsTicking;

        public override void ResetTime() {
            TimeTicked = 0;
        }
        
        public void Reset(int newTicksPerSecond) {
			TicksPerTime = newTicksPerSecond;
			ResetTime();
        }
        
        void RecalculateTimeThreshold() { 
            TimeThreshold = _ticksPerTime > 0 ? _perSeconds / _ticksPerTime : float.PositiveInfinity;
        }

        protected override void Dispose(bool disposing) {
            if (IsDisposed) return;
            if (disposing)
                OnTickEvent = null;
            base.Dispose(disposing);
        }

        public override string ToString()
        {
			return $"FreqTimer({TicksPerTime} every {PerSeconds:F2} seconds) Ticking for: {TimeTicked:F2}s)";
        }
         
    }
}
