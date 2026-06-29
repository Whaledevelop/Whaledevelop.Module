using System.Collections.Generic;

namespace Whaledevelop.Systems
{
    public interface IRuntimeSystemsContainer
    {
        IReadOnlyList<IGameSystem> Systems { get; }
    }
}
