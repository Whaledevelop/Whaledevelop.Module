namespace Whaledevelop.Actions
{
    public interface IActionsDispatcher
    {
        void Invoke<T>() where T : IAction, new();
        
        void Invoke(IAction action);

        void Invoke<TAction, TParams>(TParams actionParams)
            where TAction : IAction<TParams>, new()
            where TParams : IActionParams;
    }
}