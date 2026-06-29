using System;
using VContainer;
using Whaledevelop;

namespace Game
{
    public interface IGameModel : ILifetime
    {
        void Register(IContainerBuilder builder, Action<IGameModel> onRegistered = null, Lifetime lifetime = Lifetime.Singleton);
    }
}
