using System;
using System.Threading;
using App.InGame.Domain;
using App.InGame.Domain.Message;
using AppCore.Runtime;
using Cysharp.Threading.Tasks;
using MessagePipe;
using R3;

namespace App.InGame.Presentation.HUD
{
    public class HackingGameUseCase
    {
        private readonly CancellationTokenSource cancellationTokenSource = new();
        private readonly Subject<bool> checkPasswordSubject = new();
        private readonly ReactiveProperty<int> hackingRemainTime = new(60);
        private readonly bool pausing = false;
        private readonly Subject<string> startHackingSubject = new();
        private GameControlEntity entity;
        
        private StateMachine<HackingGameUseCase> StateMachine { get; }
        private ISubscriber<OnTriggerEnterWithGoalMessage> OnTriggerEnterWithGoalSubscriber { get; }
        private ISubscriber<OnTriggerEnterWithShieldWallMessage> OnTriggerEnterWithShieldWallSubscriber { get; }
        private IPublisher<PlayerControlPermissionMessage> PlayerControlPermissionPublisher { get; }
        private IPublisher<UnlockedShieldWallMessage> UnlockedShieldWallPublisher { get; }

        private int HackingTargetShieldWallId { get; set; }

        public ReadOnlyReactiveProperty<int> HackingRemainTime => hackingRemainTime;
        public Observable<string> StartHacking => startHackingSubject;
        public Observable<bool> CheckPassword => checkPasswordSubject;
        
        public HackingGameUseCase(IPublisher<PlayerControlPermissionMessage> playerControlPermissionPublisher,
            IPublisher<UnlockedShieldWallMessage> unlockedShieldWallPublisher,
            ISubscriber<OnTriggerEnterWithShieldWallMessage> onTriggerEnterWithShieldWallSubscriber,
            ISubscriber<OnTriggerEnterWithGoalMessage> onTriggerEnterWithGoalSubscriber)
        {
            PlayerControlPermissionPublisher = playerControlPermissionPublisher;
            UnlockedShieldWallPublisher = unlockedShieldWallPublisher;
            OnTriggerEnterWithShieldWallSubscriber = onTriggerEnterWithShieldWallSubscriber;
            OnTriggerEnterWithGoalSubscriber = onTriggerEnterWithGoalSubscriber;
            StateMachine = new StateMachine<HackingGameUseCase>(this);
            StateMachine.Change<StateInit>();
        }

        public void Setup()
        {
            //　ステージデータを取得
            entity = new GameControlEntity(60);
            entity.HackingRemainTime
                .Subscribe(time => hackingRemainTime.Value = time)
                .RegisterTo(cancellationTokenSource.Token);
        }

        public void Execute()
        {
            StateMachine.Execute();
        }

        public void Dispose()
        {
            StateMachine.Dispose();
            cancellationTokenSource.Dispose();
        }

        public void ChangeEndState()
        {
            StateMachine.Change<StateEnd>();
        }

        public void UnlockSuccess()
        {
            UnlockedShieldWallPublisher.Publish(new UnlockedShieldWallMessage(HackingTargetShieldWallId));
            PlayerControlPermissionPublisher.Publish(new PlayerControlPermissionMessage(true));
            StateMachine.Change<StateSearching>();
        }

        public void RequestPassword(string password)
        {
            checkPasswordSubject.OnNext(password == "abcd");
        }

        private void GameStart()
        {
            entity.StartCountDown(cancellationTokenSource.Token).Forget();
            PlayerControlPermissionPublisher.Publish(new PlayerControlPermissionMessage(true));
            StateMachine.Change<StateSearching>();
        }

        private string GetPassword()
        {
            // TODO 衝突した壁のIDをもとにマスタからパスワードを取得する 
            // HackingTargetShieldWallId
            return "abcd";
        }

        private class StateInit : StateMachine<HackingGameUseCase>.State
        {
            public override void Begin(HackingGameUseCase owner)
            {
                owner.OnTriggerEnterWithShieldWallSubscriber
                    .Subscribe(message => { StartHacking(owner, message.ShieldWallId); })
                    .RegisterTo(owner.cancellationTokenSource.Token);

                owner.OnTriggerEnterWithGoalSubscriber
                    .Subscribe(_ => StopCountdown(owner))
                    .RegisterTo(owner.cancellationTokenSource.Token);

                owner.StateMachine.Change<StatePreStart>();
            }

            private void StartHacking(HackingGameUseCase owner, int shieldWallId)
            {
                owner.HackingTargetShieldWallId = shieldWallId;
                owner.StateMachine.Change<StateHacking>();
            }

            private void StopCountdown(HackingGameUseCase owner)
            {
                owner.cancellationTokenSource.Cancel();
                owner.StateMachine.Change<StateEnd>();
            }
        }

        private class StatePreStart : StateMachine<HackingGameUseCase>.State
        {
            public override void Begin(HackingGameUseCase owner)
            {
                Start(owner).Forget();
            }

            private async UniTaskVoid Start(HackingGameUseCase owner)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1));
                owner.GameStart();
            }
        }

        private class StateSearching : StateMachine<HackingGameUseCase>.State
        {
            public override void Begin(HackingGameUseCase owner)
            {

            }
        }

        private class StateHacking : StateMachine<HackingGameUseCase>.State
        {
            public override void Begin(HackingGameUseCase owner)
            {
                owner.PlayerControlPermissionPublisher.Publish(new PlayerControlPermissionMessage(false));
                owner.startHackingSubject.OnNext(owner.GetPassword());
            }
        }

        private class StateEnd : StateMachine<HackingGameUseCase>.State
        {
            public override void Begin(HackingGameUseCase owner)
            {
                owner.PlayerControlPermissionPublisher.Publish(new PlayerControlPermissionMessage(false));
            }
        }
    }
}