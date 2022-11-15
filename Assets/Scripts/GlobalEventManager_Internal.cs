using System;

public static partial class GlobalEventManager
{
    private struct EventManager<T>
    {
        public event Action<T> Event;

        public void HappenedWith(T param)
        {
            Event?.Invoke(param);
        }
    }

    public struct EventManager<T1, T2>
    {
        public event Action<T1, T2> Event;

        public void HappenedWith(T1 param1, T2 param2)
        {
            Event?.Invoke(param1, param2);
        }
    }
}