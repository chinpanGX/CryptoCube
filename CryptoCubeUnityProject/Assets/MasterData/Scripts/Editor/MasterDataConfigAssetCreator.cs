using UnityEditor;
using UnityEngine;

namespace MasterData.Editor
{
    public static class MasterDataConfigAssetCreator
    {
        private static readonly string TargetFolderPath = "Assets/MasterData/EditorConfig";
        public static string[] FolderPath => new[] { TargetFolderPath };

        public static void CreateAssetIfNeeded<T>(string assetName) where T : ScriptableObject
        {
            var existingAssets = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            if (existingAssets.Length > 0)
            {
                EditorUtility.DisplayDialog("Error", $"{typeof(T).Name}アセットは作成済みです。", "OK");
                var path = AssetDatabase.GUIDToAssetPath(existingAssets[0]);
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<T>(path);
                return;
            }

            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, $"{TargetFolderPath}/{assetName}.asset");
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}