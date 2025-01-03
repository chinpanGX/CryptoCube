using AssetLoader.Application;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AppService.Runtime
{
    public static class FadeScreenExtensions
    {
        public static async UniTask<FadeScreenView> CreateAsync(IAssetLoader assetLoader)
        {
            var prefab = await assetLoader.LoadAsync<GameObject>("FadeScreenView");
            var obj = Object.Instantiate(prefab);
            obj.SetActive(false);
            return obj.GetComponentSafe<FadeScreenView>();
        }
    }
}