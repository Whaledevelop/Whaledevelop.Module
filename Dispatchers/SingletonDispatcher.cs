using VContainer;

namespace Whaledevelop
{
    public abstract class SingletonDispatcher<TSelf, TInterface>
        where TSelf : SingletonDispatcher<TSelf, TInterface>, TInterface, new()
    {
        private static TInterface _instance;

        public static TInterface Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new System.InvalidOperationException($"{typeof(TSelf).Name} is not initialized. Call CreateAndRegisterInstance first.");
                }

                return _instance;
            }
        }

        protected IObjectResolver Resolver { get; set; }

        [Inject]
        private void Construct(IObjectResolver objectResolver)
        {
            Resolver = objectResolver;
        }

        public static void CreateAndRegisterInstance(IContainerBuilder builder)
        {
            _instance = new TSelf();
            builder.RegisterInstance(_instance).As<TInterface>();
            builder.RegisterBuildCallback(resolver =>
            {
                resolver.Inject(_instance);
            });
        }
    }
}