using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Whaledevelop.Services;
using Whaledevelop.Systems;

namespace Game
{
    public partial class ScriptsGenerator
    {
        private static void WriteGeneratedFile(string filePath, string generatedCode)
        {
            var folderPath = System.IO.Path.GetDirectoryName(filePath);
            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }

            System.IO.File.WriteAllText(filePath, generatedCode);

            return;
        }

        private static bool TryGetServiceTypes(out IReadOnlyList<ServiceRegistrationData> serviceTypes)
        {
            var types = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(GetTypes)
                .Where(type => type.IsAbstract == false)
                .Where(type => typeof(IService).IsAssignableFrom(type))
                .Where(type => type.GetCustomAttribute<GameServiceAttribute>() != null)
                .OrderBy(type => type.FullName)
                .ToArray();

            var registrationData = new List<ServiceRegistrationData>();
            foreach (var type in types)
            {
                if (!TryGetGameServiceInterface(type, out var interfaceType))
                {
                    Debug.LogError($"{nameof(ScriptsGenerator)} failed to find GameService interface for {type.FullName}");
                    serviceTypes = null;

                    return false;
                }

                registrationData.Add(new ServiceRegistrationData(type, interfaceType));
            }

            serviceTypes = registrationData;

            return true;
        }

        private static bool TryGetRuntimeData(
            out IReadOnlyDictionary<string, IReadOnlyList<ModelRegistrationData>> modelTypesByScope,
            out IReadOnlyDictionary<string, IReadOnlyList<Type>> systemTypesByScope)
        {
            var scopeNames = GetScopeNames();
            var mutableModelTypesByScope = scopeNames.ToDictionary(
                scopeName => scopeName,
                _ => new List<ModelRegistrationData>());
            var mutableSystemTypesByScope = scopeNames.ToDictionary(
                scopeName => scopeName,
                _ => new List<Type>());

            var modelTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(GetTypes)
                .Where(type => type.IsAbstract == false)
                .Where(type => typeof(IGameModel).IsAssignableFrom(type))
                .Where(type => type.GetCustomAttribute<GameModelAttribute>() != null)
                .OrderBy(type => type.FullName)
                .ToArray();

            foreach (var modelType in modelTypes)
            {
                var attribute = modelType.GetCustomAttribute<GameModelAttribute>();
                if (!TryGetScopeNames(attribute.ScopeNames, modelType, out var modelScopeNames) ||
                    !TryGetGameModelInterface(modelType, out var interfaceType))
                {
                    Debug.LogError($"{nameof(ScriptsGenerator)} failed to determine scopes or interface for {modelType.FullName}");
                    modelTypesByScope = null;
                    systemTypesByScope = null;

                    return false;
                }

                foreach (var scopeName in modelScopeNames)
                {
                    var isShared = modelScopeNames.Count > 1;
                    mutableModelTypesByScope[scopeName].Add(new ModelRegistrationData(modelType, interfaceType, isShared));
                }
            }

            var systemTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(GetTypes)
                .Where(type => type.IsAbstract == false)
                .Where(type => typeof(IGameSystem).IsAssignableFrom(type))
                .Where(type => type.GetCustomAttribute<GameSystemAttribute>() != null)
                .OrderBy(type => type.FullName)
                .ToArray();

            foreach (var systemType in systemTypes)
            {
                var attribute = systemType.GetCustomAttribute<GameSystemAttribute>();
                if (!TryGetScopeNames(attribute.ScopeNames, systemType, out var systemScopeNames))
                {
                    modelTypesByScope = null;
                    systemTypesByScope = null;

                    return false;
                }

                foreach (var scopeName in systemScopeNames)
                {
                    mutableSystemTypesByScope[scopeName].Add(systemType);
                }
            }

            modelTypesByScope = mutableModelTypesByScope.ToDictionary(
                pair => pair.Key,
                pair => (IReadOnlyList<ModelRegistrationData>)pair.Value);
            systemTypesByScope = mutableSystemTypesByScope.ToDictionary(
                pair => pair.Key,
                pair => (IReadOnlyList<Type>)pair.Value);

            return true;
        }

        private static IReadOnlyList<string> GetScopeNames()
        {
            var scopeNames = new List<string>
            {
                "Game"
            };
            CollectProjectScopeNames(scopeNames);
            scopeNames.AddRange(GetAttributedScopeNames());

            return scopeNames
                .Where(scopeName => string.IsNullOrWhiteSpace(scopeName) == false)
                .Distinct()
                .ToArray();
        }

        static partial void CollectProjectScopeNames(List<string> scopeNames);

        private static IReadOnlyList<string> GetAttributedScopeNames()
        {
            var modelScopeNames = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(GetTypes)
                .Where(type => type.IsAbstract == false)
                .Select(type => type.GetCustomAttribute<GameModelAttribute>())
                .Where(attribute => attribute != null)
                .SelectMany(attribute => attribute.ScopeNames);
            var systemScopeNames = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(GetTypes)
                .Where(type => type.IsAbstract == false)
                .Select(type => type.GetCustomAttribute<GameSystemAttribute>())
                .Where(attribute => attribute != null)
                .SelectMany(attribute => attribute.ScopeNames);

            return modelScopeNames
                .Concat(systemScopeNames)
                .ToArray();
        }

        private static bool TryGetScopeNames(
            IReadOnlyList<string> scopeNames,
            Type type,
            out IReadOnlyList<string> validScopeNames)
        {
            if (scopeNames.Count == 0)
            {
                validScopeNames = new[]
                {
                    "Game"
                };

                return true;
            }

            if (scopeNames.Any(string.IsNullOrWhiteSpace) ||
                scopeNames.Distinct().Count() != scopeNames.Count)
            {
                Debug.LogError($"{nameof(ScriptsGenerator)} found invalid scope names on {type.FullName}");
                validScopeNames = null;

                return false;
            }

            validScopeNames = scopeNames;

            return true;
        }

        private static IEnumerable<Type> GetTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException exception)
            {
                return exception.Types.Where(type => type != null);
            }
        }

        private static bool TryGetGameModelInterface(Type type, out Type interfaceType)
        {
            var baseType = type.BaseType;
            while (baseType != null)
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(GameModel<,>))
                {
                    interfaceType = baseType.GetGenericArguments()[1];

                    return true;
                }

                baseType = baseType.BaseType;
            }

            interfaceType = null;

            return false;
        }

        private static bool TryGetGameServiceInterface(Type type, out Type interfaceType)
        {
            var candidates = type.GetInterfaces()
                .Where(interfaceCandidate => interfaceCandidate != typeof(IService))
                .Where(interfaceCandidate => typeof(IService).IsAssignableFrom(interfaceCandidate))
                .ToArray();

            var concreteInterfaces = candidates
                .Where(interfaceCandidate => candidates.Any(otherInterface => interfaceCandidate != otherInterface &&
                    interfaceCandidate.IsAssignableFrom(otherInterface)) == false)
                .ToArray();

            if (concreteInterfaces.Length == 1)
            {
                interfaceType = concreteInterfaces[0];

                return true;
            }

            interfaceType = null;

            return false;
        }

        private sealed class ModelRegistrationData
        {
            public ModelRegistrationData(Type concreteType, Type interfaceType, bool isShared)
            {
                ConcreteType = concreteType;
                InterfaceType = interfaceType;
                IsShared = isShared;
            }

            public Type ConcreteType { get; }

            public Type InterfaceType { get; }

            public bool IsShared { get; }
        }

        private sealed class ServiceRegistrationData
        {
            public ServiceRegistrationData(Type concreteType, Type interfaceType)
            {
                ConcreteType = concreteType;
                InterfaceType = interfaceType;
            }

            public Type ConcreteType { get; }

            public Type InterfaceType { get; }
        }
    }
}
