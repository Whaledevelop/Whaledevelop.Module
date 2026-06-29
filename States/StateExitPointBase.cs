using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Whaledevelop.States
{
    public abstract class StateExitPointBase : IStateExitPoint
    {
        private readonly IUpdatesDispatcher _updatesDispatcher;
        private readonly StateLifecycleContext _lifecycleContext;

        protected StateExitPointBase(
            IUpdatesDispatcher updatesDispatcher,
            StateLifecycleContext lifecycleContext)
        {
            _updatesDispatcher = updatesDispatcher;
            _lifecycleContext = lifecycleContext;
        }

        public async UniTask ExitAsync(CancellationToken cancellationToken)
        {
            Exception exitException = null;
            var systems = _lifecycleContext.InitializedSystems;
            for (var index = systems.Count - 1; index >= 0; index--)
            {
                var system = systems[index];
                _updatesDispatcher.TryUnregister(system);

                try
                {
                    await system.ReleaseAsync(cancellationToken);
                }
                catch (Exception exception)
                {
                    exitException ??= exception;
                }
            }

            try
            {
                await OnExitAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                exitException ??= exception;
            }

            var models = _lifecycleContext.InitializedModels;
            for (var index = models.Count - 1; index >= 0; index--)
            {
                try
                {
                    await models[index].ReleaseAsync(cancellationToken);
                }
                catch (Exception exception)
                {
                    exitException ??= exception;
                }
            }

            _lifecycleContext.Clear();

            if (exitException != null)
            {
                throw exitException;
            }
        }

        protected virtual UniTask OnExitAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}
