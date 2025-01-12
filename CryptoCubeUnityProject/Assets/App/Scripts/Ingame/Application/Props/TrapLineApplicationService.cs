using System;
using App.InGame.Domain.Message;
using MessagePipe;
using R3;
using VContainer;

namespace App.InGame.Application
{
    public class TrapLineApplicationService : IDisposable
    {
        private readonly IPublisher<OnTriggerEnterWithPlayerRestartMessage> publisher;
        private readonly CancellationDisposable cancellationDisposable = new();
        
        [Inject]
        public TrapLineApplicationService(IPublisher<OnTriggerEnterWithPlayerRestartMessage> publisher)
        {
            this.publisher = publisher;
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