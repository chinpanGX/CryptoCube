using System;
using System.Threading;
using App.InGame.Message;
using AppCore.Runtime;
using Cysharp.Threading.Tasks;
using MessagePipe;
using R3;

namespace App.InGame.HUD
{
    public class HackingTaskModel : IModel
    {
        private readonly ReactiveProperty<int> hackingRemainTime = new(10);
        private readonly Subject<string> startHackingSubject = new();
        private readonly Subject<bool> checkPasswordSubject = new();
        private readonly bool pausing = false;
        private readonly CancellationTokenSource cancellationTokenSource = new();
        private StateMachine<HackingTaskModel> StateMachine { get; }
        private IPublisher<PlayerControlPermissionMessage> PlayerControlPermissionPublisher { get; }
        private ISubscriber<OnTriggerEnterWithShieldWallMessage> OnTriggerEnterWithShieldWallSubscriber { get; }
        private IPublisher<UnlockedShieldWallMessage> UnlockedShieldWallPublisher { get; }
        
        private int HackingTargetShieldWallId { get; set; }

        public ReadOnlyReactiveProperty<int> HackingRemainTime => hackingRemainTime;
        public Observable<string> StartHacking => startHackingSubject;
        public Observable<bool> CheckPassword => checkPasswordSubject;

        public HackingTaskModel(IPublisher<PlayerControlPermissionMessage> playerControlPermissionPublisher,
            ISubscriber<OnTriggerEnterWithShieldWallMessage> onTriggerEnterWithShieldWallSubscriber,
            IPublisher<UnlockedShieldWallMessage> unlockedShieldWallPublisher)
        {
            PlayerControlPermissionPublisher = playerControlPermissionPublisher;
            OnTriggerEnterWithShieldWallSubscriber = onTriggerEnterWithShieldWallSubscriber;
            UnlockedShieldWallPublisher = unlockedShieldWallPublisher;
            StateMachine = new StateMachine<HackingTaskModel>(this);
            StateMachine.Change<StateInit>();
        }

        public void Setup()
        {
               
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

        private async UniTaskVoid CountdownTask()
        {
            while (hackingRemainTime.Value > 0)
            {
                if (cancellationTokenSource.Token.IsCancellationRequested)
                    break;

                if (pausing)
                    await UniTask.Yield(cancellationTokenSource.Token);

                await UniTask.Delay(1000, cancellationToken: cancellationTokenSource.Token);
                hackingRemainTime.Value--;
            }
        }
        
        private string GetPassword()
        {
            // TODO 衝突した壁のIDをもとにマスタからパスワードを取得する 
            // HackingTargetShieldWallId
            return "abcd";
        }

        private class StateInit : StateMachine<HackingTaskModel>.State
        {
            public override void Begin(HackingTaskModel owner)
            {
                owner.StateMachine.Change<StatePreStart>();
            }
        }

        private class StatePreStart : StateMachine<HackingTaskModel>.State
        {
            public override void Begin(HackingTaskModel owner)
            {
                Start(owner).Forget();
            }

            private async UniTaskVoid Start(HackingTaskModel owner)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1));
                owner.CountdownTask().Forget();
                owner.PlayerControlPermissionPublisher.Publish(new PlayerControlPermissionMessage(true));
                owner.StateMachine.Change<StateSearching>();
            }
        }

        private class StateSearching : StateMachine<HackingTaskModel>.State
        {
            public override void Begin(HackingTaskModel owner)
            {
                owner.OnTriggerEnterWithShieldWallSubscriber
                    .Subscribe(message =>
                        {
                            owner.StateMachine.Change<StateHacking>();
                            owner.HackingTargetShieldWallId = message.ShieldWallId;
                        }
                    )
                    .RegisterTo(owner.cancellationTokenSource.Token);
            }
        }

        private class StateHacking : StateMachine<HackingTaskModel>.State
        {
            public override void Begin(HackingTaskModel owner)
            {
                owner.PlayerControlPermissionPublisher.Publish(new PlayerControlPermissionMessage(false));
                owner.startHackingSubject.OnNext(owner.GetPassword());
            }
        }

        private class StateEnd : StateMachine<HackingTaskModel>.State
        {
            public override void Begin(HackingTaskModel owner)
            {
                UnityEngine.Debug.Log("End");
                owner.PlayerControlPermissionPublisher.Publish(new PlayerControlPermissionMessage(false));
            }
        }
    }
}