namespace Whaledevelop
{
    public interface IUpdatesDispatcher
    {
        void TryRegister<T>(T item) where T : class;

        void TryUnregister<T>(T item) where T : class;
    }
}