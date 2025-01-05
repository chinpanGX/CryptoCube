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
        private readonly CancellationDisposable cancellationDisposable = new();
        private readonly ISpawn spawn;

        public Observable<Vector3> OnRespawn => onRespawn;
        public bool CanControl { get; private set; }

        [Inject]
        public PlayerUseCase(ISubscriber<PlayerControlPermissionMessage> playerControlPermissionSubscriber,
            ISubscriber<OnTriggerEnterWithPlayerRestartMessage> onTriggerEnterWithPlayerRestartSubscriber,
            IPublisher<PlayerRespawnCompletedMessage> playerRespawnCompletedPublisher,
            ISpawn spawn)
        {
            CanControl = false;
            this.spawn = spawn;
            this.playerRespawnCompletedPublisher = playerRespawnCompletedPublisher;

            playerControlPermissionSubscriber
                .Subscribe(message => { CanControl = message.CanControl; })
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
            CanControl = true;
        }
        
        private Vector3 GetSpawnPosition()
        {
            CanControl = false;
            return spawn.GetSpawnPosition();
        }
    }
}