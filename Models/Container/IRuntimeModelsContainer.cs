using System.Collections.Generic;

namespace Game
{
    public interface IRuntimeModelsContainer
    {
        IReadOnlyList<IGameModel> Models { get; }
    }
}
