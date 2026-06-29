using VContainer;

namespace Whaledevelop.Actions
{
    public sealed class ActionsDispatcher : SingletonDispatcher<ActionsDispatcher, IActionsDispatcher>, IActionsDispatcher
    {
        public void Invoke<T>() where T : IAction, new()
        {
            var action = new T();
            Resolver.Inject(action);
            action.Invoke();
        }

        public void Invoke(IAction action)
        {
            Resolver.Inject(action);
            action.Invoke();
        }

        public void Invoke<TAction, TParams>(TParams actionParams)
            where TAction : IAction<TParams>, new()
            where TParams : IActionParams
        {
            var action = new TAction();
            Resolver.Inject(action);
            action.Invoke(actionParams);
        }
    }
}