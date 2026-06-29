using System.Collections.Generic;

namespace Whaledevelop.Services
{
    public sealed class RuntimeServicesContainer : IRuntimeServicesContainer
    {
        private readonly List<IService> _services = new();

        public IReadOnlyList<IService> Services => _services;

        public void SetServices(IReadOnlyList<IService> services)
        {
            _services.Clear();
            _services.AddRange(services);

            return;
        }
    }
}
