using System;

namespace Whaledevelop
{
    public interface IUpdateCallbacks
    {
        event Action OnUpdate;
        
        event Action OnLateUpdate;
        
        event Action OnFixedUpdate;
    }
}