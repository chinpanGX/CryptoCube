using System;
using App.InGame.Domain.Message;
using MessagePipe;
using R3;
using VContainer;

namespace App.InGame.Application
{
    public class PatrolEnemyApplicationService : IDisposable
    {
        private readonly IPublisher<OnTriggerEnterWithPlayerRestartMessage> publisher;
        private readonly Subject<Unit> onRestart = new();
        private readonly CancellationDisposable cancellationDisposable = new();
        public Observable<Unit> OnRestart => onRestart;

        [Inject]
        public PatrolEnemyApplicationService(
            IPublisher<OnTriggerEnterWithPlayerRestartMessage> publisher,
            ISubscriber<PlayerRespawnCompletedMessage> playerRespawnCompletedSubscriber)
        {
            this.publisher = publisher;
            playerRespawnCompletedSubscriber
                .Subscribe(_ => onRestart.OnNext(Unit.Default))
                .RegisterTo(cancellationDisposable.Token);
        }

        public void OnTriggerEnterPublish()
        {
            publisher.Publish(new OnTriggerEnterWithPlayerRestartMessage());
        }
        
        public void Dispose()
        {
            cancellationDisposable?.Dispose();
        }
    }
}