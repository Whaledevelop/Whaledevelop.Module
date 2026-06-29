namespace Whaledevelop.Actions
{
    public interface IAction
    {
        void Invoke();
    }

    public interface IAction<T> where T : IActionParams
    {
        void Invoke(T actionParams);
    }
}