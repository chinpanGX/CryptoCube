using System;
using System.Threading;
using App.InGame.Application;
using Cysharp.Threading.Tasks;
using R3;
using R3.Triggers;
using VContainer;
using UnityEngine;
using UnityEngine.InputSystem;

namespace App.InGame.Presentation.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private float speed = 5.0f;

        private Vector3 direction;
        private InputAction moveAction;
        private CameraController cameraController;

        private PlayerUseCase PlayerUseCase { get; set; }

        [Inject]
        public void Construct(PlayerUseCase playerUseCase, ICameraSwitcher cameraSwitcher)
        {
            PlayerUseCase = playerUseCase;
            cameraController = new CameraController(cameraSwitcher,
                playerInput.actions["LeftLock"],
                playerInput.actions["RightLock"]
            );

            playerInput.DeactivateInput();

            moveAction = playerInput.actions["Move"];
            moveAction.performed += DoMove;
            moveAction.canceled += CancelMove;

            this.UpdateAsObservable()
                .Subscribe(_ => Tick())
                .RegisterTo(destroyCancellationToken);

            PlayerUseCase.OnRespawn
                .SubscribeAwait(async (spawnPosition, ct) => await RespawnAsync(spawnPosition, ct))
                .RegisterTo(destroyCancellationToken);

            PlayerUseCase.CanControl
                .Subscribe(x =>
                {
                    if (x)
                    {
                        playerInput.ActivateInput();
                    }
                    else
                    {
                        playerInput.DeactivateInput();
                    }
                })
                .RegisterTo(destroyCancellationToken);
        }

        private void Tick()
        {
            characterController.Move(direction * (speed * Time.deltaTime));
        }

        private void DoMove(InputAction.CallbackContext context)
        {
            var move = context.ReadValue<Vector2>();
            direction = new Vector3(move.x, 0, move.y);
        }

        private void CancelMove(InputAction.CallbackContext context)
        {
            direction = Vector3.zero;
        }

        private async UniTask RespawnAsync(Vector3 spawnPosition, CancellationToken ct)
        {
            transform.position = spawnPosition;
            await UniTask.Delay(500, cancellationToken: ct);
            PlayerUseCase.RespawnCompleted();
        }

        private void OnDestroy()
        {
            moveAction.performed -= DoMove;
            moveAction.canceled -= CancelMove;
            cameraController.Dispose();
            PlayerUseCase.Dispose();
        }

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
    }
}