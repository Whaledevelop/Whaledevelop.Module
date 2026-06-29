namespace Whaledevelop.UI
{
    public interface IUIView
    {
        void Release();
    }

    public interface IUIView<TModel> : IUIView where TModel : IUIViewModel
    {
        void Initialize(TModel model);
    }
}
