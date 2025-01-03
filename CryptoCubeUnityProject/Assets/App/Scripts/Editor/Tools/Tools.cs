using UnityEditor;
using UnityEngine;

namespace App.Editor.Tools
{
    public static class HyperLinks
    {
        [MenuItem("ツール/マスタデータ")]
        public static void OpenMasterData()
        {
            Application.OpenURL("https://drive.google.com/drive/u/0/folders/1Xpe5Kvr9St7JNaaLCt-1_cJTy1ifkcIM");
        }
    }
}
