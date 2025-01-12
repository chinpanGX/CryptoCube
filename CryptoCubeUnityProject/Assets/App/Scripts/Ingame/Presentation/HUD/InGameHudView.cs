using System.Collections.Generic;
using App.AppServiceExtensions;
using AppCore.Runtime;
using AppService.Runtime;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

namespace App.InGame.Presentation.HUD
{
    public class InGameHudView : MonoBehaviour, IView
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private CustomText remainingTimeText;
        [Header("アニメーション")]
        [SerializeField] private List<PlayableGroup> playableGroup;
        [SerializeField] private PlayableDirector playableDirector;
        
        private readonly Dictionary<string, PlayableAsset> playableAssetDictionary = new();
        
        public Canvas Canvas => canvas;
        private static ViewScreen ViewScreen => ComponentLocator.Get<ViewScreen>();

        public void Push()
        {
            ViewScreen.Push(this);
            foreach (var group in playableGroup)
            {
                playableAssetDictionary[group.Key] = group.PlayableAsset;        
            }
        }

        public void Pop()
        {
            ViewScreen.Pop();
        }

        public void Open()
        {
            gameObject.SetActive(true);
            PlayAnimation("ManualOpen").Forget();
        }

        public void Close()
        {
            Destroy(gameObject);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetRemainingTimeText(int remainingTime)
        {
            remainingTimeText.SetTextSafe($"{remainingTime}");
        }
        
        public async UniTask PlayAnimation(string key)
        {
            playableDirector.playableAsset = playableAssetDictionary[key];
            playableDirector.Play();
            await UniTask.WaitUntil(() => playableDirector.state != PlayState.Playing);
        }
    }
}