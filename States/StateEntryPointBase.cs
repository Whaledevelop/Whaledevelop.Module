using System.Threading;
using Cysharp.Threading.Tasks;
using Game;
using Whaledevelop.Systems;

namespace Whaledevelop.States
{
    public abstract class StateEntryPointBase : IStateEntryPoint
    {
        private readonly IRuntimeModelsContainer _modelsContainer;
        private readonly IRuntimeSystemsContainer _systemsContainer;
        private readonly IUpdatesDispatcher _updatesDispatcher;
        private readonly StateLifecycleContext _lifecycleContext;

        protected StateEntryPointBase(
            IRuntimeModelsContainer modelsContainer,
            IRuntimeSystemsContainer systemsContainer,
            IUpdatesDispatcher updatesDispatcher,
            StateLifecycleContext lifecycleContext)
        {
            _modelsContainer = modelsContainer;
            _systemsContainer = systemsContainer;
            _updatesDispatcher = updatesDispatcher;
            _lifecycleContext = lifecycleContext;
        }

        public async UniTask EnterAsync(CancellationToken cancellationToken)
        {
            foreach (var model in _modelsContainer.Models)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await model.InitializeAsync(cancellationToken);
                _lifecycleContext.AddModel(model);
            }

            cancellationToken.ThrowIfCancellationRequested();
            await OnEnterAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var system in _systemsContainer.Systems)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await system.InitializeAsync(cancellationToken);
                _updatesDispatcher.TryRegister(system);
                _lifecycleContext.AddSystem(system);
            }
        }

        protected virtual UniTask OnEnterAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}
