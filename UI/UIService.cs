using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Whaledevelop.UI
{
    public abstract class UIService<TEnum> : SyncLifetime, IUIService<TEnum> where TEnum : struct
    {
        private readonly Dictionary<TEnum, UIViewRuntimeData> _viewRuntimeDatas = new();

        private IObjectResolver _objectResolver;

        [Inject]
        private void Construct(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        protected abstract TView CreateView<TView>(TEnum code) where TView : IUIView;

        protected abstract void DestroyView(TEnum code, IUIView view);

        public bool TryGetModel<TModel>(TEnum code, out TModel viewModel) where TModel : IUIViewModel
        {
            if (_viewRuntimeDatas.TryGetValue(code, out var runtimeData) &&
                runtimeData.TryGetModel(out TModel model))
            {
                viewModel = model;

                return true;
            }

            viewModel = default;

            return false;
        }

        public bool TryOpenView<TModel, TView>(TEnum code, TModel model) where TModel : IUIViewModel where TView : IUIView<TModel>
        {
            if (_viewRuntimeDatas.TryGetValue(code, out var runtimeData))
            {
                Debug.Log($"Already opened {code}");

                return false;
            }

            _objectResolver.Inject(model);
            model.Initialize();
            var view = CreateView<TView>(code);
            if (view == null)
            {
                Debug.LogError($"View creation failed for {code}");
                model.Release();

                return false;
            }

            view.Initialize(model);
            runtimeData = new UIViewRuntimeData(view, model);
            _viewRuntimeDatas.Add(code, runtimeData);

            return true;
        }

        public bool TryClose<TModel, TView>(TEnum code) where TModel : IUIViewModel where TView : IUIView<TModel>
        {
            if (!_viewRuntimeDatas.TryGetValue(code, out var runtimeData))
            {
                Debug.Log($"Already closed {code}");

                return false;
            }

            runtimeData.View.Release();

            if (runtimeData.TryGetModel(out TModel model))
            {
                model.Release();
            }

            DestroyView(code, runtimeData.View);
            _viewRuntimeDatas.Remove(code);

            return true;
        }
    }
}
