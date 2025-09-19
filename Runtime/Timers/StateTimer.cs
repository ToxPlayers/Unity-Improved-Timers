using System;

namespace ScaledTimers {
    public class StateTimer : StateTimer<bool> { }

    [Serializable]
    public class StateTimer<T> : StopwatchTimer
    {
        T _curValue;
        public T Value
        {
            get => _curValue;
            set
            {
                var tmp = _curValue;
                _curValue = value;
                if ( ! _curValue.Equals(tmp) )
                    Reset();
            }
        }
    }
}