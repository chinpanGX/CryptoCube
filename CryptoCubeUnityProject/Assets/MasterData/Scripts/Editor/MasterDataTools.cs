using System;
using UnityEditor;
using UnityEngine;

namespace MasterData.Editor
{
    public static class MasterDataTools
    {
        [MenuItem("ツール/マスタデータ/ドライブを開く", false, 100)]
        public static void OpenMasterDataDrive()
        {
            var guilds =
                AssetDatabase.FindAssets("t:GoogleSpreadSheetsLinkAsset", MasterDataConfigAssetCreator.FolderPath);
            if (guilds.Length <= 0)
            {
                EditorUtility.DisplayDialog("Error", "GoogleSpreadSheetsLinkAssetが見つかりませんでした", "OK");
                return;
            }

            var path = AssetDatabase.GUIDToAssetPath(guilds[0]);
            var link = AssetDatabase.LoadAssetAtPath<GoogleSpreadSheetsLinkAsset>(path).GoogleDriveLink;
            if (string.IsNullOrEmpty(link))
            {
                EditorUtility.DisplayDialog("Error", "リンクが入力されていません", "OK");
                return;
            }
            Application.OpenURL(link);
        }
    }
}