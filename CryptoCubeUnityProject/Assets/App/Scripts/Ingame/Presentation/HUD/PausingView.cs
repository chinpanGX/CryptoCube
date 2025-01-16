using System;
using System.Collections.Generic;
using App.AppServiceExtensions;
using AppCore.Runtime;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Playables;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace App.InGame.Presentation.HUD
{
    public class PausingView : MonoBehaviour, IView
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private PlayableAsset openingPlayableAsset;
        [SerializeField] private PlayableAsset closingPlayableAsset;
        [SerializeField] private PlayableDirector playableDirector;
        public Canvas Canvas => canvas;
        private static ModalScreen ModalScreen => ComponentLocator.Get<ModalScreen>();

        public static async UniTask<AsyncOperationHandle<GameObject>> LoadAsync()
        {
            var handle = Addressables.LoadAssetAsync<GameObject>("PausingView");
            await handle.ToUniTask();
            return handle;
        }
        
        public static PausingView Create(AsyncOperationHandle<GameObject> handle)
        {
            var go = Instantiate(handle.Result);
            var comp = go.GetComponent<PausingView>(); 
            comp.Push();
            return comp;
        }

        public void Push()
        {
            ModalScreen.Push(this);
            gameObject.SetActive(true);
            PlayAsync(openingPlayableAsset).Forget();
        }

        public void Pop()
        {
            
        }

        public void Open()
        {
            
        }

        public void Close()
        {
            
        }
        
        public async UniTask CloseAsync()
        {
            await PlayAsync(closingPlayableAsset);
            ModalScreen.Pop();
            Destroy(gameObject);
        }

        private async UniTask PlayAsync(PlayableAsset playableAsset)
        {
            playableDirector.playableAsset = playableAsset;
            await playableDirector.PlayAsync(destroyCancellationToken);
        }
    }
}