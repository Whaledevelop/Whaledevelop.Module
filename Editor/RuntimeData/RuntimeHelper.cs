using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;
using Whaledevelop.VContainer;

namespace Game
{
    public partial class RuntimeHelper : OdinEditorWindow
    {
        [ShowInInspector]
        [ToggleGroup(nameof(_showGameScope), "$GameScopeLabel")]
#pragma warning disable CS0414
        private bool _showGameScope = true;
#pragma warning restore CS0414

        [ShowInInspector]
        [ToggleGroup(nameof(_showGameScope))]
        [LabelText("Services")]
        [ListDrawerSettings(DefaultExpandedState = true)]
        private IReadOnlyList<RuntimeServiceDebugRow> GameServices => CreateServiceRows(FindGameScope());

        [ShowInInspector]
        [ToggleGroup(nameof(_showGameScope))]
        [LabelText("Systems")]
        [ListDrawerSettings(DefaultExpandedState = true)]
        private IReadOnlyList<RuntimeSystemDebugRow> GameSystems => CreateSystemRows(FindGameScope());

        [ShowInInspector]
        [ToggleGroup(nameof(_showGameScope))]
        [LabelText("Models")]
        [ListDrawerSettings(DefaultExpandedState = true)]
        private IReadOnlyList<RuntimeModelDebugRow> GameModels => CreateModelRows(FindGameScope());

        [ShowInInspector]
        [ToggleGroup(nameof(_showSceneScope), "$SceneScopeLabel")]
#pragma warning disable CS0414
        private bool _showSceneScope = true;
#pragma warning restore CS0414

        [ShowInInspector]
        [ToggleGroup(nameof(_showSceneScope))]
        [LabelText("Services")]
        [ListDrawerSettings(DefaultExpandedState = true)]
        private IReadOnlyList<RuntimeServiceDebugRow> SceneServices => CreateServiceRows(FindSceneScope());

        [ShowInInspector]
        [ToggleGroup(nameof(_showSceneScope))]
        [LabelText("Systems")]
        [ListDrawerSettings(DefaultExpandedState = true)]
        private IReadOnlyList<RuntimeSystemDebugRow> SceneSystems => CreateSystemRows(FindSceneScope());

        [ShowInInspector]
        [ToggleGroup(nameof(_showSceneScope))]
        [LabelText("Models")]
        [ListDrawerSettings(DefaultExpandedState = true)]
        private IReadOnlyList<RuntimeModelDebugRow> SceneModels => CreateModelRows(FindSceneScope());

        private string GameScopeLabel => CreateScopeLabel("Game Scope", FindGameScope());

        private string SceneScopeLabel => CreateScopeLabel("Scene Scope", FindSceneScope());

        [MenuItem("Whaledevelop/Runtime Helper")]
        private static void Open()
        {
            var window = GetWindow<RuntimeHelper>();
            window.titleContent = new GUIContent("Runtime Helper");
            window.Show();

            return;
        }

        private static LifetimeScope FindGameScope()
        {
            var lifetimeScopes = FindObjectsByType<LifetimeScope>(FindObjectsInactive.Include);

            return lifetimeScopes.FirstOrDefault(IsGameScope);
        }

        private static LifetimeScope FindSceneScope()
        {
            var activeScene = SceneManager.GetActiveScene();
            var lifetimeScopes = FindObjectsByType<LifetimeScope>(FindObjectsInactive.Include);

            return lifetimeScopes.FirstOrDefault(lifetimeScope =>
                !IsGameScope(lifetimeScope) &&
                lifetimeScope.gameObject.scene == activeScene);
        }

        private static bool IsGameScope(LifetimeScope lifetimeScope)
        {
            var type = lifetimeScope.GetType();
            while (type != null)
            {
                if (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(GameLifetimeScopeBase<>))
                {

                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }

        private static string CreateScopeLabel(string label, LifetimeScope lifetimeScope)
        {
            if (!lifetimeScope)
            {
                return $"{label} (Not Found)";
            }

            return $"{label} ({lifetimeScope.GetType().Name})";
        }
    }
}
