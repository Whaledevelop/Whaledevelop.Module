using System;

namespace Whaledevelop
{
    public interface IGameCycleCallbacks
    {
        event Action OnDestroyEvent;
        
        event Action OnApplicationQuitEvent;
    }
}