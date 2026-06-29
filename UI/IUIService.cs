using Whaledevelop.Services;

namespace Whaledevelop.UI
{
    public interface IUIService<TEnum> : IService where TEnum : struct
    {
        bool TryGetModel<TModel>(TEnum code, out TModel viewModel) where TModel : IUIViewModel;

        bool TryOpenView<TModel, TView>(TEnum code, TModel model)
            where TModel : IUIViewModel where TView : IUIView<TModel>;

        bool TryClose<TModel, TView>(TEnum code) where TModel : IUIViewModel where TView : IUIView<TModel>;
    }
}
