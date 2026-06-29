using System.Collections.Generic;

namespace Whaledevelop.Systems
{
    public sealed class RuntimeSystemsContainer : IRuntimeSystemsContainer
    {
        private readonly List<IGameSystem> _systems = new();

        public IReadOnlyList<IGameSystem> Systems => _systems;

        public void SetSystems(IReadOnlyList<IGameSystem> systems)
        {
            _systems.Clear();
            _systems.AddRange(systems);

            return;
        }
    }
}
