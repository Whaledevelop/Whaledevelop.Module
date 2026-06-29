using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Whaledevelop
{
    public class ComponentsPool<T> where T : Component
    {
        private readonly ObjectPool<T> _pool;
        private readonly List<T> _active = new();

        public IReadOnlyList<T> Active => _active;

        public ComponentsPool(T prefab, Transform parent, int prewarm = 0, int maxSize = 32)
        {
            _pool = new ObjectPool<T>(
                () => Object.Instantiate(prefab, parent),
                OnGet,
                OnRelease,
                OnDestroy,
                true,
                prewarm,
                maxSize
            );

            for (var i = 0; i < prewarm; i++)
            {
                var obj = _pool.Get();
                _pool.Release(obj);
            }
        }

        public T Get()
        {
            var component = _pool.Get();
            _active.Add(component);

            return component;
        }
        
        public void Return(T item)
        {
            _pool.Release(item);
            _active.Remove(item);
        }
        
        public void ReturnAll(Action<T> callback = null)
        {
            foreach (var item in _active)
            {
                callback?.Invoke(item);
                _pool.Release(item);
            }

            _active.Clear();
        }

        public void Clear()
        {
            ReturnAll();
            _pool.Clear();
        }

        private void OnGet(T component)
        {
            component.gameObject.SetActive(true);
        }

        private void OnRelease(T component)
        {
            component.gameObject.SetActive(false);
        }

        private void OnDestroy(T component)
        {
            if (component)
            {
                Object.Destroy(component.gameObject);
            }
        }
    }
}