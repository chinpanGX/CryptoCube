using UnityEditor;
using UnityEngine;

namespace MasterData.Editor
{
    public class GoogleSpreadSheetsLinkAsset : ScriptableObject
    {
        [SerializeField] private string googleDriveLink;
        public string GoogleDriveLink => googleDriveLink;

        [MenuItem("ツール/マスタデータ/ScriptableObjects/Google Spread Sheets Link Assetを作成", false, 1)]
        public static void CreateGoogleSpreadSheetsLinkAsset()
        {
            MasterDataConfigAssetCreator.CreateAssetIfNeeded<GoogleSpreadSheetsLinkAsset>("GoogleSpreadSheetsLink");
        }
    }
}