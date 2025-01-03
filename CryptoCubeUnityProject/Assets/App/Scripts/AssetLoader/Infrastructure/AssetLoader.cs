using System;
using System.Collections.Generic;
using AssetLoader.Application;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace AssetLoader.Infrastructure
{
    public class AssetLoader : IAssetLoader
    {
        private static readonly AssetCache cache = new();

        public async UniTask<T> LoadAsync<T>(string assetName) where T : Object
        {
            if (cache.TryGet(assetName, out T asset))
            {
                return asset;
            }

            var handle = Addressables.LoadAssetAsync<T>(assetName);
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Failed)
            {
                throw new AssetLoadException($"Failed to load asset: {assetName}");
            }
            cache.Add(assetName, handle.Result);
            return handle.Result;
        }

        public void Dispose()
        {
            cache.Clear();
        }
    }

    internal class AssetCache
    {
        private readonly Dictionary<string, Object> cache = new();

        public bool TryGet<T>(string assetName, out T asset) where T : Object
        {
            if (cache.TryGetValue(assetName, out var obj))
            {
                asset = obj as T;
                return true;
            }
            asset = null;
            return false;
        }

        public void Add(string assetName, Object asset)
        {
            cache[assetName] = asset;
        }

        public void Clear()
        {
            cache.Clear();
        }
    }

    public class AssetLoadException : Exception
    {
        public AssetLoadException(string message) : base(message)
        {
        }
    }
}