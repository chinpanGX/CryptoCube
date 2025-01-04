using System;
using App.InGame.Message;
using App.InGame.Prop;
using Cysharp.Threading.Tasks;
using MessagePipe;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace App.InGame.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour, IDisposable
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private float speed = 5.0f;

        private bool canControl;
        private Vector3 direction;
        private InputAction moveAction;

        private ISubscriber<PlayerControlPermissionMessage> playerControlPermissionSubscriber;

        public void Dispose()
        {
            moveAction.performed -= DoMove;
            moveAction.canceled -= CancelMove;
        }

        [Inject]
        public void Construct(ISubscriber<PlayerControlPermissionMessage> subscriber,
            ISubscriber<OnTriggerEnterWithPatrolEnemyMessage> onTriggerEnterWithPatrolEnemySubscriber,
            ICharacterSpawner characterSpawner)
        {
            canControl = false;
            playerControlPermissionSubscriber = subscriber;
            moveAction = playerInput.actions["Move"];
            moveAction.performed += DoMove;
            moveAction.canceled += CancelMove;

            this.UpdateAsObservable()
                .Where(_ => canControl)
                .Subscribe(_ => Tick())
                .RegisterTo(destroyCancellationToken);

            playerControlPermissionSubscriber
                .Subscribe(message => { canControl = message.CanControl; })
                .RegisterTo(destroyCancellationToken);

            onTriggerEnterWithPatrolEnemySubscriber
                .AsObservable()
                .ToObservable()
                .SubscribeAwait(async (_, ct) => 
                {
                    canControl = false;
                    transform.position = characterSpawner.Position;
                    await UniTask.Delay(500, cancellationToken: ct);
                    canControl = true;
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

        private async UniTask Respawn()
        {
            canControl = false;
            
            transform.position = Vector3.zero;
            Debug.Log("PlayerController: Player is respawned.");
        }
    }
}