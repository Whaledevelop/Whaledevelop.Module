using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Game
{
    public partial class ScriptsGenerator : OdinEditorWindow
    {
        private const string ServicesGeneratedFileName = "GameServicesComposition.Generated.cs";
        private const string RuntimeDataFileSuffix = "RuntimeDataComposition.Generated.cs";

        [BoxGroup("Generation")]
        [FolderPath(AbsolutePath = false)]
        [SerializeField]
        private string _generatedFilesFolderPath = "Assets/_game/Scripts/Generated";

        [MenuItem("Whaledevelop/ScriptsGenerator")]
        private static void Open()
        {
            var window = GetWindow<ScriptsGenerator>();
            window.titleContent = new GUIContent("ScriptsGenerator");

            return;
        }

        [Button]
        [BoxGroup("Generation")]
        private void GenerateCompositions()
        {
            if (!TryGetServiceTypes(out var serviceTypes) ||
                !TryGetRuntimeData(out var modelTypesByScope, out var systemTypesByScope))
            {
                return;
            }

            WriteGeneratedFile(System.IO.Path.Combine(_generatedFilesFolderPath, ServicesGeneratedFileName), GenerateServicesCode(serviceTypes));
            foreach (var scopeName in GetScopeNames())
            {
                var fileName = $"{scopeName}{RuntimeDataFileSuffix}";
                var generatedCode = GenerateRuntimeDataCode(
                    scopeName,
                    modelTypesByScope[scopeName],
                    systemTypesByScope[scopeName]);
                WriteGeneratedFile(System.IO.Path.Combine(_generatedFilesFolderPath, fileName), generatedCode);
            }

            AssetDatabase.Refresh();
            Debug.Log($"{nameof(ScriptsGenerator)} generated runtime data compositions");

            return;
        }
    }
}
