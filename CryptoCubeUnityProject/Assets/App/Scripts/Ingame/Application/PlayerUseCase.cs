using System;
using App.InGame.Domain;
using App.InGame.Domain.Message;
using MessagePipe;
using R3;
using UnityEngine;
using VContainer;

namespace App.InGame.Application
{
    public class PlayerUseCase : IDisposable
    {
        private readonly IPublisher<PlayerRespawnCompletedMessage> playerRespawnCompletedPublisher;
        private readonly Subject<Vector3> onRespawn = new();
        private readonly ReactiveProperty<bool> canControl = new(false);
        private readonly CancellationDisposable cancellationDisposable = new();
        private readonly ISpawn spawn;

        public Observable<Vector3> OnRespawn => onRespawn;
        public Observable<bool> CanControl => canControl;

        [Inject]
        public PlayerUseCase(ISubscriber<PlayerControlPermissionMessage> playerControlPermissionSubscriber,
            ISubscriber<OnTriggerEnterWithPlayerRestartMessage> onTriggerEnterWithPlayerRestartSubscriber,
            IPublisher<PlayerRespawnCompletedMessage> playerRespawnCompletedPublisher,
            ISpawn spawn)
        {
            canControl.Value = false;
            this.spawn = spawn;
            this.playerRespawnCompletedPublisher = playerRespawnCompletedPublisher;

            playerControlPermissionSubscriber
                .Subscribe(message => { canControl.Value = message.CanControl; })
                .RegisterTo(cancellationDisposable.Token);

            onTriggerEnterWithPlayerRestartSubscriber
                .Subscribe(_ => onRespawn.OnNext(GetSpawnPosition()))
                .RegisterTo(cancellationDisposable.Token);
        }
        
        public void Dispose()
        {
            cancellationDisposable?.Dispose();
        }
        
        public void RespawnCompleted()
        {
            playerRespawnCompletedPublisher.Publish(new PlayerRespawnCompletedMessage());
            canControl.Value = true;
        }
        
        private Vector3 GetSpawnPosition()
        {
            canControl.Value = false;
            return spawn.GetSpawnPosition();
        }
    }
}