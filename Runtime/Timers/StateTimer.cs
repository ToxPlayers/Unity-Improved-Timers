using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ScaledTimers {

    [Serializable]
    public class StateTimer : StateTimer<bool> { }

    [Serializable]
    public class StateTimer<T> : StopwatchTimer
    {
        T _curValue;
        [NonSerialized] public Func<T,T,bool> CustomComparer;
        [ShowInInspector] public T State
        {
            get => _curValue;
            set
            {
                var prevValue = _curValue;
                _curValue = value;

                bool isEqual = CustomComparer != null ?
                    CustomComparer(_curValue, prevValue) :
                    _curValue.Equals(prevValue);
                if ( ! isEqual )
                    Reset();
            }
        }
        public StateTimer() : base() {  }

        static public implicit operator T(StateTimer<T> w) => w.State;
        public override string ToString()
        {
            return $"StateTimer({State} for {TimeTicked:F2})s)";
        }
    }
}