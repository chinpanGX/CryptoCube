using App.InGame.Message;
using App.InGame.Player;
using AppCore.Runtime;
using MessagePipe;
using R3;
using R3.Triggers;
using UnityEngine;

namespace App.InGame.HUD
{
    public class InGameDirector : MonoBehaviour, IDirector
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private HackingTaskView hackingTaskView;
        [SerializeField] private InGameHudView inGameHudView;
        [SerializeField] private ShieldWallDirector shieldWallDirector;
        private UpdatablePresenter updatablePresenter;
        private PresenterFactoryProvider presenterFactoryProvider;

        void Start()
        {
            presenterFactoryProvider = ServiceLocator.TryGet<PresenterFactoryProvider>(out var value)
                ? value
                : new PresenterFactoryProvider();

            var builder = new BuiltinContainerBuilder();
            builder.AddMessagePipe();
            builder.AddMessageBroker<PlayerControlPermissionMessage>();
            builder.AddMessageBroker<OnTriggerEnterWithShieldWallMessage>();
            var provider = builder.BuildServiceProvider();
            GlobalMessagePipe.SetProvider(provider);

            updatablePresenter = new UpdatablePresenter();
            this.UpdateAsObservable()
                .Subscribe(_ => updatablePresenter.Execute())
                .RegisterTo(destroyCancellationToken);

            presenterFactoryProvider.TryAdd("InGame", new InGamePresenterFactory(this, hackingTaskView, inGameHudView));
            playerController.Construct(GlobalMessagePipe.GetSubscriber<PlayerControlPermissionMessage>());
            shieldWallDirector.Construct(GlobalMessagePipe.GetPublisher<OnTriggerEnterWithShieldWallMessage>(),
                GlobalMessagePipe.GetPublisher<PlayerControlPermissionMessage>()
            );

            Push("InGame");
        }

        void OnDestroy()
        {
            playerController.Dispose();
        }

        public async void Push(string name)
        {
            var request = await presenterFactoryProvider.Get(name).CreateAsync();
            updatablePresenter.SetRequest(request);
        }

        private class InGamePresenterFactory : IPresenterFactory
        {
            private readonly IDirector director;
            private readonly HackingTaskView hackingTaskView;
            private readonly InGameHudView inGameHudView;

            public InGamePresenterFactory(IDirector director, HackingTaskView hackingTaskView, InGameHudView inGameHudView)
            {
                this.director = director;
                this.hackingTaskView = hackingTaskView;
                this.inGameHudView = inGameHudView;
            }

            public async Awaitable<IPresenter> CreateAsync()
            {
                var cancellationDisposable = new CancellationDisposable();
                var model = new HackingTaskModel(
                    GlobalMessagePipe.GetPublisher<PlayerControlPermissionMessage>(),
                    GlobalMessagePipe.GetSubscriber<OnTriggerEnterWithShieldWallMessage>()
                );
                var presenter = new HackingPresenter(director, model, inGameHudView, hackingTaskView);
                return presenter;
            }
        }
    }

}