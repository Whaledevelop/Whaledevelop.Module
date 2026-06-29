using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using VContainer;
using VContainer.Unity;
using Whaledevelop.Services;
using Whaledevelop.Systems;

namespace Game
{
    public partial class RuntimeHelper
    {
        private static IReadOnlyList<RuntimeServiceDebugRow> CreateServiceRows(LifetimeScope lifetimeScope)
        {
            if (!TryResolveLocal<IRuntimeServicesContainer>(lifetimeScope, out var servicesContainer))
            {
                return null;
            }

            return servicesContainer.Services
                .Select(service => new RuntimeServiceDebugRow(service))
                .ToArray();
        }

        private static IReadOnlyList<RuntimeSystemDebugRow> CreateSystemRows(LifetimeScope lifetimeScope)
        {
            if (!TryResolveLocal<IRuntimeSystemsContainer>(lifetimeScope, out var systemsContainer))
            {
                return null;
            }

            return systemsContainer.Systems
                .Select(system => new RuntimeSystemDebugRow(system))
                .ToArray();
        }

        private static IReadOnlyList<RuntimeModelDebugRow> CreateModelRows(LifetimeScope lifetimeScope)
        {
            if (!TryResolveLocal<IRuntimeModelsContainer>(lifetimeScope, out var modelsContainer))
            {
                return null;
            }

            return modelsContainer.Models
                .Select(model => new RuntimeModelDebugRow(model))
                .ToArray();
        }

        private static bool TryResolveLocal<T>(LifetimeScope lifetimeScope, out T instance)
        {
            instance = default;

            if (!lifetimeScope ||
                lifetimeScope.Container == null ||
                !lifetimeScope.Container.TryGetRegistration(typeof(T), out var registration))
            {
                return false;
            }

            instance = (T)lifetimeScope.Container.Resolve(registration);

            return true;
        }

        private sealed class RuntimeServiceDebugRow
        {
            private readonly IService _service;

            public RuntimeServiceDebugRow(IService service)
            {
                _service = service;
            }

            [ShowInInspector]
            public string Type => _service.GetType().Name;

            [ShowInInspector]
            [HideReferenceObjectPicker]
            public object Data => _service;
        }

        private sealed class RuntimeSystemDebugRow
        {
            private readonly IGameSystem _system;

            public RuntimeSystemDebugRow(IGameSystem system)
            {
                _system = system;
            }

            [ShowInInspector]
            public string Type => _system.GetType().Name;

            [ShowInInspector]
            [HideReferenceObjectPicker]
            public object Data => _system;
        }

        private sealed class RuntimeModelDebugRow
        {
            private readonly IGameModel _model;

            public RuntimeModelDebugRow(IGameModel model)
            {
                _model = model;
            }

            [ShowInInspector]
            public string Type => _model.GetType().Name;

            [ShowInInspector]
            [HideReferenceObjectPicker]
            public object Data => _model;
        }
    }
}
