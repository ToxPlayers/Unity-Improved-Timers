using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ScaledTimers {
    public class StateTimer : StateTimer<bool> { }

    [Serializable]
    public class StateTimer<T> : StopwatchTimer
    {
        T _curValue;
        public Func<T,T,bool> CustomComparer;
         
        [ShowInInspector] public T Value
        {
            get => _curValue;
            set
            {
                var prevValue = _curValue;
                _curValue = value;

                bool isEqual = CustomComparer != null ?
                    CustomComparer(_curValue, prevValue) :
                    ! _curValue.Equals(prevValue);
                if ( ! isEqual )
                    Reset();
            }
        }
        public StateTimer() : base() {  }

        static public implicit operator T(StateTimer<T> w) => w.Value;
        public override string ToString()
        {
            return $"StateTimer({Value} for {TimeRunning:F2})s)";
        }
    }
}