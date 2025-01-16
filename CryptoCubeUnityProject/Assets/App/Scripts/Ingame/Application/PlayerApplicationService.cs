using System;
using App.InGame.Domain;
using App.InGame.Domain.Message;
using MessagePipe;
using R3;
using UnityEngine;
using VContainer;

namespace App.InGame.Application
{
    public class PlayerApplicationService : IDisposable
    {
        private readonly IPublisher<PauseMessage> pausePublisher;
        private readonly IPublisher<PlayerRespawnCompletedMessage> playerRespawnCompletedPublisher;
        private readonly Subject<Vector3> onRespawn = new();
        private readonly ReactiveProperty<bool> canControl = new(false);
        private readonly CancellationDisposable cancellationDisposable = new();
        private readonly ISpawn spawn;
        private bool isPause;

        public Observable<Vector3> OnRespawn => onRespawn;
        public ReadOnlyReactiveProperty<bool> CanControl => canControl;

        [Inject]
        public PlayerApplicationService(ISubscriber<PlayerControlPermissionMessage> playerControlPermissionSubscriber,
            ISubscriber<OnTriggerEnterWithPlayerRestartMessage> onTriggerEnterWithPlayerRestartSubscriber,
            IPublisher<PlayerRespawnCompletedMessage> playerRespawnCompletedPublisher,
            IPublisher<PauseMessage> pausePublisher,
            ISpawn spawn)
        {
            isPause = false;
            canControl.Value = false;
            this.spawn = spawn;
            this.playerRespawnCompletedPublisher = playerRespawnCompletedPublisher;
            this.pausePublisher = pausePublisher;

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
        
        public void ChangePause()
        {
            isPause = !isPause;
            pausePublisher.Publish(new PauseMessage(isPause));
        }
        
        private Vector3 GetSpawnPosition()
        {
            canControl.Value = false;
            return spawn.GetSpawnPosition();
        }
    }
}