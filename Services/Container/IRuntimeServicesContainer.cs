using System.Collections.Generic;

namespace Whaledevelop.Services
{
    public interface IRuntimeServicesContainer
    {
        IReadOnlyList<IService> Services { get; }
    }
}
