using System;
using System.Collections.Generic;

namespace Game
{
    public sealed class SharedGameModelsContainer : ISharedGameModelsContainer
    {
        private readonly Dictionary<Type, IGameModel> _models = new();

        public TModel GetOrCreate<TModel>(Func<TModel> createModel) where TModel : class, IGameModel
        {
            var modelType = typeof(TModel);
            if (_models.TryGetValue(modelType, out var existingModel))
            {
                return (TModel)existingModel;
            }

            var model = createModel();
            _models.Add(modelType, model);

            return model;
        }
    }
}
