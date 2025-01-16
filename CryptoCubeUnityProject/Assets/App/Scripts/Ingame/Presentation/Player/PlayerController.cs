using System;
using System.Collections.Generic;
using System.Threading;
using App.InGame.Application;
using Cysharp.Threading.Tasks;
using R3;
using R3.Triggers;
using VContainer;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

namespace App.InGame.Presentation.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("アニメーション")] [SerializeField] private Animator animator;
        [SerializeField] private Transform model;

        [Space(10)] [SerializeField] private CharacterController characterController;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private float speed = 5.0f;

        private AnimationController animationController;
        private CameraController cameraController;
        private MoveController moveController;

        private PlayerApplicationService PlayerApplicationService { get; set; }

        [Inject]
        public void Construct(PlayerApplicationService playerApplicationService, ICameraSwitcher cameraSwitcher)
        {
            PlayerApplicationService = playerApplicationService;
            cameraController = new CameraController(cameraSwitcher,
                playerInput.actions["LeftLock"],
                playerInput.actions["RightLock"]
            );

            animationController = new AnimationController(animator);
            moveController = new MoveController(model, animationController, characterController,
                playerInput.actions["Move"],
                speed
            );

            this.UpdateAsObservable()
                .Subscribe(_ => Tick())
                .RegisterTo(destroyCancellationToken);

            PlayerApplicationService.OnRespawn
                .SubscribeAwait(async (spawnPosition, ct) => await RespawnAsync(spawnPosition, ct))
                .RegisterTo(destroyCancellationToken);

            PlayerApplicationService.CanControl
                .Subscribe(SetActivateInput)
                .RegisterTo(destroyCancellationToken);

            SetActivateInput(false);
            animationController.Play("Idle");
        }

        private void Tick()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                PlayerApplicationService.ChangePause();
            }

            if (PlayerApplicationService.CanControl.CurrentValue)
            {
                moveController.Tick();
            }
        }

        private void SetActivateInput(bool canControl)
        {
            if (canControl)
            {
                playerInput.ActivateInput();
                return;
            }

            playerInput.DeactivateInput();
        }

        private async UniTask RespawnAsync(Vector3 spawnPosition, CancellationToken ct)
        {
            moveController.ForceStop();
            transform.position = spawnPosition;
            await UniTask.Delay(500, cancellationToken: ct);
            PlayerApplicationService.RespawnCompleted();
        }

        private void OnDestroy()
        {
            animationController.Dispose();
            moveController.Dispose();
            cameraController.Dispose();
            PlayerApplicationService.Dispose();
        }

        #region MoveController

        class MoveController : IDisposable
        {
            private readonly Transform model;
            private readonly AnimationController animationController;
            private readonly CharacterController characterController;
            private readonly InputAction moveAction;
            private readonly float speed;
            private Vector3 direction;

            public MoveController(
                Transform model,
                AnimationController animationController,
                CharacterController characterController,
                InputAction moveAction, float speed)
            {
                this.model = model;
                this.animationController = animationController;
                this.characterController = characterController;
                this.moveAction = moveAction;
                this.moveAction.performed += DoMove;
                this.moveAction.canceled += CancelMove;
                this.speed = speed;
                direction = Vector3.zero;
            }

            public void Tick()
            {
                characterController.Move(direction * (speed * Time.deltaTime));
                if (direction != Vector3.zero)
                {
                    animationController.Play("Running");
                    model.rotation = Quaternion.LookRotation(direction);
                }
            }

            public void ForceStop()
            {
                direction = Vector3.zero;
            }

            public void Dispose()
            {
                moveAction.performed -= DoMove;
                moveAction.canceled -= CancelMove;
            }

            private void DoMove(InputAction.CallbackContext context)
            {
                var move = context.ReadValue<Vector2>();
                direction = new Vector3(move.x, 0, move.y);
            }

            private void CancelMove(InputAction.CallbackContext context)
            {
                direction = Vector3.zero;
                animationController.Play("Idle");
            }
        }

        #endregion MoveController

        #region CameraController

        class CameraController : IDisposable
        {
            private readonly ICameraSwitcher cameraSwitcher;
            private readonly InputAction leftAction;
            private readonly InputAction rightAction;

            public CameraController(ICameraSwitcher cameraSwitcher, InputAction leftAction, InputAction rightAction)
            {
                this.cameraSwitcher = cameraSwitcher;
                this.leftAction = leftAction;
                this.rightAction = rightAction;

                this.leftAction.performed += DoLeftLock;
                this.leftAction.canceled += DoCancel;
                this.rightAction.performed += DoRightLock;
                this.rightAction.canceled += DoCancel;
            }

            private void DoLeftLock(InputAction.CallbackContext context)
            {
                cameraSwitcher.SwitchCamera(ActivateCamera.LeftLock);
            }

            private void DoRightLock(InputAction.CallbackContext context)
            {
                cameraSwitcher.SwitchCamera(ActivateCamera.RightLock);
            }

            private void DoCancel(InputAction.CallbackContext context)
            {
                cameraSwitcher.SwitchCamera(ActivateCamera.ForwardLock);
            }

            public void Dispose()
            {
                leftAction.performed -= DoLeftLock;
                leftAction.canceled -= DoCancel;
                rightAction.performed -= DoRightLock;
                rightAction.canceled -= DoCancel;
            }
        }

        #endregion CameraController

        #region AnimationController

        class AnimationController : IDisposable
        {
            private readonly Animator animator;

            public AnimationController(Animator animator)
            {
                this.animator = animator;

            }

            public void Play(string key)
            {
                animator.SetBool("Running", key != "Idle");
            }

            public void Dispose()
            {

            }
        }

        #endregion AnimationController

    }
}