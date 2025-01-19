using System.IO;
using UnityEditor;
using UnityEngine;

namespace App.Scripts.User.Editor
{
    internal static class UserMenuItem
    {
        [MenuItem("ツール/セーブデータ/エクスプローラーを開く")]
        public static void OpenSaveDataExplorer()
        { 
            // windowsの場合
            // パスの区切り文字がスラッシュだとエクスプローラーが開けないため, バックスラッシュに置換している
            var path = Application.persistentDataPath.Replace("/", "\\");
            System.Diagnostics.Process.Start("explorer.exe", $"{path}");
        }
        
        [MenuItem("ツール/セーブデータ/削除")]
        public static void DeleteSaveData()
        {
            if (File.Exists($"{Application.persistentDataPath}/user.json"))
            {
                File.Delete($"{Application.persistentDataPath}/user.json");
            }
        }
    }
}