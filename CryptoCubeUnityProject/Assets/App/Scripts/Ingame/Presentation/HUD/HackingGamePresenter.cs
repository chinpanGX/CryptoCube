using AppCore.Runtime;
using Cysharp.Threading.Tasks;
using R3;
using VContainer;

namespace App.InGame.Presentation.HUD
{
    public class HackingGamePresenter : IPresenter
    {
        private readonly CancellationDisposable cancellationDisposable;
        private IDirector Director { get; set; }
        private InGameHudView InGameHudView { get; set; }
        private HackingGameView HackingGameView { get; set; }
        private HackingGameApplicationService ApplicationService { get; set; }

        [Inject]
        public HackingGamePresenter(IDirector director, HackingGameApplicationService applicationService,
            InGameHudView inGameHudView,
            HackingGameView hackingGameView)
        {
            Director = director;
            ApplicationService = applicationService;
            InGameHudView = inGameHudView;
            HackingGameView = hackingGameView;
            cancellationDisposable = new CancellationDisposable();
            Setup();
        }

        public void Execute()
        {
            ApplicationService.Execute();
        }

        public void Dispose()
        {
            Director = null;
            InGameHudView.Pop();
            InGameHudView = null;
            HackingGameView.Pop();
            HackingGameView = null;
            ApplicationService.Dispose();
            ApplicationService = null;
            cancellationDisposable.Dispose();
        }

        private void Setup()
        {
            HackingGameView.Push();
            HackingGameView.Hide();

            InGameHudView.Push();
            InGameHudView.Hide();

            ApplicationService.HackingRemainTime
                .Subscribe(InGameHudView.SetRemainingTimeText)
                .RegisterTo(cancellationDisposable.Token);

            ApplicationService.StartHacking
                .Subscribe(StartHacking)
                .RegisterTo(cancellationDisposable.Token);

            ApplicationService.HackingRemainTime
                .Where(x => x == 0)
                .Subscribe(_ => EndState())
                .RegisterTo(cancellationDisposable.Token);

            ApplicationService.CheckPassword
                .SubscribeAwait(async (x, _) =>
                    {
                        CheckPassword(x);
                        await InGameHudView.PlayAnimation("ManualOpen");
                    }
                )
                .RegisterTo(cancellationDisposable.Token);

            HackingGameView.Request
                .Subscribe(text => ApplicationService.RequestPassword(text))
                .RegisterTo(cancellationDisposable.Token);

            ApplicationService.PreStart
                .Subscribe(_ => { OnStart().Forget(); })
                .RegisterTo(cancellationDisposable.Token);
            HackingGameView.Setup();
            InGameHudView.Open();
        }

        private void StartHacking(string passwordAnswer)
        {
            InGameHudView.PlayAnimation("ManualClose").Forget();
            HackingGameView.PreOpenSetup(passwordAnswer);
            HackingGameView.Open();
        }

        private void CheckPassword(bool passwordCorrect)
        {
            if (!passwordCorrect) return;

            HackingGameView.Hide();
            ApplicationService.ShieldUnlockedSuccess();
        }

        private void EndState()
        {
            ApplicationService.ChangeEndState();
            InGameHudView.Close();
        }

        private async UniTask OnStart()
        {
            var view = await StartView.LoadAsync();
            await view.PlayAsync();
            ApplicationService.GameStart();
        }
    }
}