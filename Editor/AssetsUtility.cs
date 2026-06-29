using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Whaledevelop
{
    public static class AssetsUtility
    {
        public static List<(TView view, string path)> LoadPrefabsWithComponent<TView>(string folderPath) where TView : Component
        {
            var result = new List<(TView view, string path)>();
            var guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }
                var asset = AssetDatabase.LoadAssetAtPath<TView>(path);
                if (asset == null)
                {
                    continue;
                }
                result.Add((asset, path));
            }

            return result;
        }
    }
}