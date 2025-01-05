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
        
        private PlayerUseCase PlayerUseCase { get; set; }
        
        [Inject]
        public void Construct(PlayerUseCase playerUseCase)
        {
            PlayerUseCase = playerUseCase;
            moveAction = playerInput.actions["Move"];
            moveAction.performed += DoMove;
            moveAction.canceled += CancelMove;

            this.UpdateAsObservable()
                .Where(_ => PlayerUseCase.CanControl)
                .Subscribe(_ => Tick())
                .RegisterTo(destroyCancellationToken);

            PlayerUseCase.OnRespawn
                .SubscribeAwait(async (spawnPosition, ct) => await RespawnAsync(spawnPosition, ct))
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
            PlayerUseCase.Dispose();
        }
    }
}