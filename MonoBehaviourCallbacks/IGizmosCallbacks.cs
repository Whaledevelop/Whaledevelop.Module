using System;

namespace Whaledevelop
{
    public interface IGizmosCallbacks
    {
        event Action OnDrawGizmosEvent;
    }
}