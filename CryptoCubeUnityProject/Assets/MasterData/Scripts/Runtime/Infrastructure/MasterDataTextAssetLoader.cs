using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MasterData.Runtime.Infrastructure
{
    public partial class MasterDataLoader
    {
        private static readonly string MasterDataPath = "Assets/MasterData/Resource/";
        private static readonly string MasterDataExtension = ".json";

        private async UniTask<T[]> LoadAsync<T>(string masterDataName)
        {
            var assetKey = $"{MasterDataPath}{masterDataName}{MasterDataExtension}";
            var asset = await Addressables.LoadAssetAsync<TextAsset>(assetKey).ToUniTask();
            if (!asset.text.StartsWith("{\"Root\":"))
            {
                throw new MasterDataException("Invalid master data format.");
            }
            var master = JsonUtility.FromJson<MasterDataElement<T>>(asset.text);
            return master.Root;
        }

        [Serializable]
        private class MasterDataElement<T>
        {
            public T[] Root;
        }
    }

    public class MasterDataException : Exception
    {
        public MasterDataException(string message) : base(message)
        {
        }
    }
}