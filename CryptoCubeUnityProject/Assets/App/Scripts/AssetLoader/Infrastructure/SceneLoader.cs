using AssetLoader.Application;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AssetLoader.Infrastructure
{
    public class SceneLoader : ISceneLoader
    {
        public async UniTask LoadSceneAsync(string sceneName, bool isSingle)
        {
            await SceneManager.LoadSceneAsync(sceneName, isSingle ? LoadSceneMode.Single : LoadSceneMode.Additive);
        }
        
        public async UniTask UnloadSceneAsync(string sceneName)
        {
            await SceneManager.UnloadSceneAsync(sceneName);
        }
    }
}
