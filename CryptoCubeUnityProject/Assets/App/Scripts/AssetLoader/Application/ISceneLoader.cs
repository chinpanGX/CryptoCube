using Cysharp.Threading.Tasks;

namespace AssetLoader.Application
{
    public interface ISceneLoader
    {
        UniTask LoadSceneAsync(string sceneName, bool isSingle);
        UniTask UnloadSceneAsync(string sceneName);
    }
}