using System;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

namespace AssetLoader.Application
{
    public interface IAssetLoader : IDisposable
    {
        UniTask<T> LoadAsync<T>(string assetName) where T : Object;
    }

}
