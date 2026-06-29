using System;

namespace Game
{
    public interface ISharedGameModelsContainer
    {
        TModel GetOrCreate<TModel>(Func<TModel> createModel) where TModel : class, IGameModel;
    }
}
