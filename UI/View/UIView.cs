using UnityEngine;

namespace Whaledevelop.UI
{

    public abstract class UIView : MonoBehaviour, IUIView
    {
        public abstract void Release();
    }

    public abstract class UIView<TModel> : UIView, IUIView<TModel> where TModel : IUIViewModel
    {
        public abstract void Initialize(TModel model);
    }
}
