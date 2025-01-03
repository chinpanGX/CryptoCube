using System.IO;
using UnityEditor;
using UnityEngine;

namespace MasterData.Editor
{
    public static class MasterDataFormatter
    {
        [MenuItem("ツール/マスタデータ/jsonを整形する")]
        public static void Execute()
        {
            var resourcePath = Path.Combine(Application.dataPath, "MasterData/Resource").Replace("\\", "/");
            var jsonFiles = Directory.GetFiles(resourcePath, "*.json");

            var isWrite = false;
            foreach (var file in jsonFiles)
            {
                var jsonData = File.ReadAllText(file);
                if (jsonData.StartsWith("{\"Root\":")) continue;
                var newJsonData = "{\"Root\":[" + jsonData.Trim().TrimStart('[').TrimEnd(']') + "]}";
                newJsonData = newJsonData.Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace(" ", "");
                File.WriteAllText(file, newJsonData);
                isWrite = true;
            }

            if (isWrite)
            {
                EditorUtility.DisplayDialog("Success", "Jsonファイルにルート要素を追加して上書き保存しました。", "OK");
                AssetDatabase.Refresh();
            }
        }
    }
}