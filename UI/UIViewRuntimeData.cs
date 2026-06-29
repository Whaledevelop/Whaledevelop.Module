namespace Whaledevelop.UI
{
    public class UIViewRuntimeData
    {
        public UIViewRuntimeData(IUIView view, IUIViewModel viewModel)
        {
            View = view;
            ViewModel = viewModel;
        }

        public IUIView View { get; }

        private IUIViewModel ViewModel { get; }

        public bool TryGetModel<TModel>(out TModel model) where TModel : IUIViewModel
        {
            if (ViewModel is TModel typedModel)
            {
                model = typedModel;

                return true;
            }

            model = default;

            return false;
        }
    }
}
