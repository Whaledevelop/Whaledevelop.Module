#if UNITY_EDITOR
using System.Collections.Generic;

namespace Game
{
    public static class RuntimeModelsDebugContainer
    {
        public static IReadOnlyList<IGameModel> Models { get; private set; }

        public static void SetModels(IReadOnlyList<IGameModel> models)
        {
            Models = models;

            return;
        }

        public static void Clear()
        {
            Models = null;

            return;
        }
    }
}
#endif
