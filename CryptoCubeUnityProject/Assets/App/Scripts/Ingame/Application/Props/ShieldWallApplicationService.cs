using System;
using App.InGame.Domain.Message;
using MessagePipe;
using R3;
using VContainer;

namespace App.InGame.Application
{
    public class ShieldWallApplicationService : IDisposable
    {
        private readonly IPublisher<OnTriggerEnterWithShieldWallMessage> onTriggerEnterWithShieldWallPublisher;
        private readonly Subject<int> onUnlock = new();
        private readonly CancellationDisposable destroyCancellationToken = new();
        public Observable<int> OnUnlock => onUnlock;

        [Inject]
        public ShieldWallApplicationService(
            IPublisher<OnTriggerEnterWithShieldWallMessage> onTriggerEnterWithShieldWallPublisher,
            ISubscriber<UnlockedShieldWallMessage> unlockedShieldWallSubscriber)
        {
            this.onTriggerEnterWithShieldWallPublisher = onTriggerEnterWithShieldWallPublisher;

            unlockedShieldWallSubscriber.Subscribe(
                message => { onUnlock.OnNext(message.ShieldWallId); }
            ).RegisterTo(destroyCancellationToken.Token);
        }

        public void OnTriggerEnterPublish(int shieldWallId)
        {
            onTriggerEnterWithShieldWallPublisher.Publish(new OnTriggerEnterWithShieldWallMessage(shieldWallId));
        }

        public void Dispose()
        {
            destroyCancellationToken.Dispose();
        }
    }
}