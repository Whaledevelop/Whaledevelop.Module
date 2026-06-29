#if UNITY_EDITOR
using System.Collections.Generic;

namespace Whaledevelop.Services
{
    public static class RuntimeServicesDebugContainer
    {
        public static IReadOnlyList<IService> Services { get; private set; }

        public static void SetServices(IReadOnlyList<IService> services)
        {
            Services = services;

            return;
        }

        public static void Clear()
        {
            Services = null;

            return;
        }
    }
}
#endif
