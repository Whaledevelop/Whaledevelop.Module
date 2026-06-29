#if UNITY_EDITOR
using System.Collections.Generic;

namespace Whaledevelop.Systems
{
    public static class RuntimeSystemsDebugContainer
    {
        public static IReadOnlyList<IGameSystem> Systems { get; private set; }

        public static void SetSystems(IReadOnlyList<IGameSystem> systems)
        {
            Systems = systems;

            return;
        }

        public static void Clear()
        {
            Systems = null;

            return;
        }
    }
}
#endif
