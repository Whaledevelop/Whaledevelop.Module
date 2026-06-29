using System;
using VContainer;
using Whaledevelop;

namespace Game
{
    public abstract class GameModel<TClass, TInterface> : SyncLifetime, IGameModel
    {
        public void Register(IContainerBuilder builder, Action<IGameModel> onRegistered = null, Lifetime lifetime = Lifetime.Singleton)
        {
            builder.Register<TClass>(lifetime).As<TInterface>();
            builder.RegisterBuildCallback(resolver =>
            {
                var runtimeModel = resolver.Resolve<TInterface>() as IGameModel;
                if (runtimeModel == null)
                {
                    return;
                }

                onRegistered?.Invoke(runtimeModel);
            });

            return;
        }
    }
}
