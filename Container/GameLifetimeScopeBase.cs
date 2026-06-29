using VContainer;
using VContainer.Unity;
using Whaledevelop.Actions;

namespace Whaledevelop.VContainer
{
    public abstract class GameLifetimeScopeBase<TEntryPoint> : LifetimeScope where TEntryPoint : EntryPointBase
    {
        private MonoBehaviourCallbacks _callbacksInstance;
        
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            
            RegisterMonoBehaviourCallbacks(builder);

            RegisterDefaultDispatchers(builder);
            
            RegisterEntryPoint(builder);
        }
        
        protected override void OnDestroy()
        {
            if (_callbacksInstance)
            {
                Destroy(_callbacksInstance.gameObject);
                _callbacksInstance = null;
            }

            base.OnDestroy();
        }
        
        private void RegisterMonoBehaviourCallbacks(IContainerBuilder builder)
        {
            _callbacksInstance = MonoBehaviourCallbacks.CreateAndRegister(builder);
        }

        private void RegisterDefaultDispatchers(IContainerBuilder builder)
        {
            ActionsDispatcher.CreateAndRegisterInstance(builder);
            EventsDispatcher.CreateAndRegisterInstance(builder);
            UpdatesDispatcher.CreateAndRegisterInstance(builder);
        }

        private void RegisterEntryPoint(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<TEntryPoint>();
        }
    }
}
