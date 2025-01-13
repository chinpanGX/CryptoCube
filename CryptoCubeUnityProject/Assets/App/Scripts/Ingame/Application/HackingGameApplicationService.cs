using System;
using System.Threading;
using App.InGame.Domain;
using App.InGame.Domain.Message;
using AppCore.Runtime;
using Cysharp.Threading.Tasks;
using MessagePipe;
using R3;
using VContainer;

namespace App.InGame.Presentation.HUD
{
    public class HackingGameApplicationService
    {
        private readonly CancellationTokenSource cancellationTokenSource = new();
        private readonly Subject<bool> checkPasswordSubject = new();
        private readonly Subject<string> startHackingSubject = new();
        private readonly ReactiveProperty<int> hackingRemainTime = new(60);
        private readonly ReactiveProperty<bool> pausing = new(false);
        private readonly ReactiveProperty<Unit> preStart = new();
        private GameControlEntity entity;
        
        public Observable<string> StartHacking => startHackingSubject;
        public Observable<bool> CheckPassword => checkPasswordSubject;
        public ReadOnlyReactiveProperty<int> HackingRemainTime => hackingRemainTime;
        public ReadOnlyReactiveProperty<bool> Pausing => pausing;
        public ReadOnlyReactiveProperty<Unit> PreStart => preStart;
        
        private StateMachine<HackingGameApplicationService> StateMachine { get; }
        private ISubscriber<OnTriggerEnterWithGoalMessage> OnTriggerEnterWithGoalSubscriber { get; }
        private ISubscriber<OnTriggerEnterWithShieldWallMessage> OnTriggerEnterWithShieldWallSubscriber { get; }
        private IPublisher<PlayerControlPermissionMessage> PlayerControlPermissionPublisher { get; }
        private IPublisher<UnlockedShieldWallMessage> UnlockedShieldWallPublisher { get; }
        
        private int HackingTargetShieldWallId { get; set; }
        
        [Inject]
        public HackingGameApplicationService(IPublisher<PlayerControlPermissionMessage> playerControlPermissionPublisher,
            IPublisher<UnlockedShieldWallMessage> unlockedShieldWallPublisher,
            ISubscriber<OnTriggerEnterWithShieldWallMessage> onTriggerEnterWithShieldWallSubscriber,
            ISubscriber<OnTriggerEnterWithGoalMessage> onTriggerEnterWithGoalSubscriber)
        {
            PlayerControlPermissionPublisher = playerControlPermissionPublisher;
            UnlockedShieldWallPublisher = unlockedShieldWallPublisher;
            OnTriggerEnterWithShieldWallSubscriber = onTriggerEnterWithShieldWallSubscriber;
            OnTriggerEnterWithGoalSubscriber = onTriggerEnterWithGoalSubscriber;
            StateMachine = new StateMachine<HackingGameApplicationService>(this);
            StateMachine.Change<StateInit>();
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
        
        public void GameStart()
        {
            // ゲーム開始
            entity.StartCountDown(cancellationTokenSource.Token).Forget();
            PlayerControlPermissionPublisher.Publish(new PlayerControlPermissionMessage(true));
            StateMachine.Change<StateSearching>();
        }
        
        public void ShieldUnlockedSuccess()
        {
            UnlockedShieldWallPublisher.Publish(new UnlockedShieldWallMessage(HackingTargetShieldWallId));
            PlayerControlPermissionPublisher.Publish(new PlayerControlPermissionMessage(true));
            StateMachine.Change<StateSearching>();
        }

        public void RequestPassword(string password)
        {
            checkPasswordSubject.OnNext(password == "abcd");
        }
        
        private void Setup()
        {
            //　ステージデータを取得
            entity = new GameControlEntity(60);
            entity.HackingRemainTime
                .Subscribe(time => hackingRemainTime.Value = time)
                .RegisterTo(cancellationTokenSource.Token);
        }
        
        private string GetPassword()
        {
            // TODO 衝突した壁のIDをもとにマスタからパスワードを取得する 
            // HackingTargetShieldWallId
            return "abcd";
        }
        
        #region StateMachine
        private class StateInit : StateMachine<HackingGameApplicationService>.State
        {
            public override void Begin(HackingGameApplicationService owner)
            {
                owner.PlayerControlPermissionPublisher.Publish(new PlayerControlPermissionMessage(false));
                
                owner.OnTriggerEnterWithShieldWallSubscriber
                    .Subscribe(message => { StartHacking(owner, message.ShieldWallId); })
                    .RegisterTo(owner.cancellationTokenSource.Token);

                owner.OnTriggerEnterWithGoalSubscriber
                    .Subscribe(_ => StopCountdown(owner))
                    .RegisterTo(owner.cancellationTokenSource.Token);
                
                owner.Setup();
            }

            private void StartHacking(HackingGameApplicationService owner, int shieldWallId)
            {
                owner.HackingTargetShieldWallId = shieldWallId;
                owner.StateMachine.Change<StateHacking>();
            }

            private void StopCountdown(HackingGameApplicationService owner)
            {
                owner.cancellationTokenSource.Cancel();
                owner.StateMachine.Change<StateEnd>();
            }
        }
        
        private class StateSearching : StateMachine<HackingGameApplicationService>.State
        {
            public override void Begin(HackingGameApplicationService owner)
            {

            }
        }

        private class StateHacking : StateMachine<HackingGameApplicationService>.State
        {
            public override void Begin(HackingGameApplicationService owner)
            {
                owner.PlayerControlPermissionPublisher.Publish(new PlayerControlPermissionMessage(false));
                owner.startHackingSubject.OnNext(owner.GetPassword());
            }
        }

        private class StateEnd : StateMachine<HackingGameApplicationService>.State
        {
            public override void Begin(HackingGameApplicationService owner)
            {
                owner.PlayerControlPermissionPublisher.Publish(new PlayerControlPermissionMessage(false));
            }
        }
        #endregion StateMachine
    }
}