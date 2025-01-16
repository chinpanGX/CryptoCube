using System.Collections.Generic;
using App.AppServiceExtensions;
using AppCore.Runtime;
using AppService.Runtime;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace App.InGame.Presentation.HUD
{
    public class InGameHudView : MonoBehaviour, IView
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private CustomText remainingTimeText;
        [Header("アニメーション")] [SerializeField] private List<PlayableGroup> playableGroup;
        [SerializeField] private PlayableDirector playableDirector;

        [Header("PlayerInput")] [SerializeField]
        private PlayerInput playerInput;

        private readonly Dictionary<string, PlayableAsset> playableAssetDictionary = new();
        private readonly Subject<Unit> onPushedPause = new();

        public Observable<Unit> OnPushedPause => onPushedPause;
        public Canvas Canvas => canvas;
        private static ViewScreen ViewScreen => ComponentLocator.Get<ViewScreen>();
        private InputAction pauseAction;
        private AsyncOperationHandle<GameObject> pauseObjectHandle;
        private PausingView pausingView;

        public void Push()
        {
            if (playerInput == null)
            {
                Debug.LogError("PlayerInput is null");
                return;
            }

            ViewScreen.Push(this);
            foreach (var group in playableGroup)
            {
                playableAssetDictionary[group.Key] = group.PlayableAsset;
            }
            pauseAction = playerInput.actions["Pause"];
            pauseAction.performed += OnPushPause;
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
            await UniTask.WaitUntil(() => playableDirector.state != PlayState.Playing,
                cancellationToken: destroyCancellationToken
            );
        }

        private void OnPushPause(InputAction.CallbackContext context)
        {
            onPushedPause.OnNext(Unit.Default);
        }

        public async UniTask OnPauseView(bool isPausing)
        {
            if (isPausing)
            {
                pauseObjectHandle = await PausingView.LoadAsync();
                pausingView = PausingView.Create(pauseObjectHandle);
            }
            else
            {
                if (pausingView == null)
                    return;

                await pausingView.CloseAsync();
            }
        }

        void OnDestroy()
        {
            pauseAction.performed -= OnPushPause;
            if (pauseObjectHandle.IsValid())
                pauseObjectHandle.Release();
        }
    }
}