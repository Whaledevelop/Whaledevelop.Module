using System.Collections.Generic;
using Game;
using Whaledevelop.Systems;

namespace Whaledevelop.States
{
    public sealed class StateLifecycleContext
    {
        private readonly List<IGameModel> _initializedModels = new();
        private readonly List<IGameSystem> _initializedSystems = new();

        public IReadOnlyList<IGameModel> InitializedModels => _initializedModels;

        public IReadOnlyList<IGameSystem> InitializedSystems => _initializedSystems;

        public void AddModel(IGameModel model)
        {
            _initializedModels.Add(model);
        }

        public void AddSystem(IGameSystem system)
        {
            _initializedSystems.Add(system);
        }

        public void Clear()
        {
            _initializedModels.Clear();
            _initializedSystems.Clear();
        }
    }
}
