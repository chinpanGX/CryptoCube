using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using MasterData.Runtime.Application;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MasterData.Runtime.Infrastructure
{
    public class MasterDataLoader : IMasterDataLoader
    {
        public async UniTask<T[]> LoadAsync<T>(string assetName)
        {
            var asset = await Addressables.LoadAssetAsync<TextAsset>(assetName).ToUniTask();
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
}