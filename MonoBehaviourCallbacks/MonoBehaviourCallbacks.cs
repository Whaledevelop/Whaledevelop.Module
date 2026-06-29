using System;
using UnityEngine;
using VContainer;

namespace Whaledevelop
{
    public class MonoBehaviourCallbacks : MonoBehaviour, IGameCycleCallbacks, IUpdateCallbacks, IGizmosCallbacks
    {
        public event Action OnUpdate;
        
        public event Action OnLateUpdate;
        
        public event Action OnFixedUpdate;

        public event Action OnDestroyEvent;
        
        public event Action OnApplicationQuitEvent;

        public event Action OnDrawGizmosEvent;
        
        public static MonoBehaviourCallbacks CreateAndRegister(IContainerBuilder builder)
        {
            var callbackGameObject = new GameObject("MonoBehaviourCallbacks");
            var callbacksInstance = callbackGameObject.AddComponent<MonoBehaviourCallbacks>();
           DontDestroyOnLoad(callbackGameObject);

            builder.RegisterInstance(callbacksInstance)
                .As<IUpdateCallbacks>()
                .As<IGizmosCallbacks>()
                .As<IGameCycleCallbacks>();
            return callbacksInstance;
        }

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            OnLateUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }

        private void OnDestroy()
        {
            OnDestroyEvent?.Invoke();
        }

        private void OnApplicationQuit()
        {
            OnApplicationQuitEvent?.Invoke();
        }

        private void OnDrawGizmos()
        {
            OnDrawGizmosEvent?.Invoke();
        }
    }
}