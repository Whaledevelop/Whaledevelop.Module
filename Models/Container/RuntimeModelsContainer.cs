using System.Collections.Generic;

namespace Game
{
    public sealed class RuntimeModelsContainer : IRuntimeModelsContainer
    {
        private readonly List<IGameModel> _models = new();

        public IReadOnlyList<IGameModel> Models => _models;

        public void SetModels(IReadOnlyList<IGameModel> models)
        {
            _models.Clear();
            _models.AddRange(models);

            return;
        }
    }
}
