using App.AppServiceExtensions;
using AppCore.Runtime;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Playables;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace App.InGame.Presentation.HUD
{
    internal class StartView : MonoBehaviour, IView
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private PlayableDirector playableDirector;
        public Canvas Canvas => canvas;
        private static ModalScreen ModalScreen => ComponentLocator.Get<ModalScreen>();
        private AsyncOperationHandle<GameObject> handle;

        public static async UniTask<StartView> LoadAsync()
        {
            var handle = Addressables.LoadAssetAsync<GameObject>("StartView");
            await handle.Task;
            var go = Instantiate(handle.Result);
            var comp = go.GetComponent<StartView>(); 
            comp.handle = handle;
            comp.Push();
            return comp;
        }
        
        public async UniTask PlayAsync()
        {
            await playableDirector.PlayAsync();
            Pop();
        }
        
        public void Push()
        {
            ModalScreen.Push(this);
            gameObject.SetActive(true);
        }
        
        public void Pop()
        {
            ModalScreen.Pop();
            handle.Release();
        }
        
        public void Open()
        {
            
        }
        
        public void Close()
        {
            Destroy(gameObject);   
        }
    }
}