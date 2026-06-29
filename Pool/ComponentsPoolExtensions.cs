using System;
using UnityEngine;

namespace Whaledevelop
{
    public static class ComponentsPoolExtensions
    {
        public static bool TryGetValue<T>(this ComponentsPool<T> pool, Func<T, bool> predicate, out T value) where T: Component
        {
            value = null;
            foreach (var item in pool.Active)
            {
                if (!predicate(item))
                {
                    continue;
                }
                value = item;
                return true;
            }
            return false;
        }
        
        public static bool HasValue<T>(this ComponentsPool<T> pool, Func<T, bool> predicate) where T: Component
        {
            foreach (var item in pool.Active)
            {
                if (!predicate(item))
                {
                    continue;
                }
                return true;
            }
            return false;
        }
    }
}