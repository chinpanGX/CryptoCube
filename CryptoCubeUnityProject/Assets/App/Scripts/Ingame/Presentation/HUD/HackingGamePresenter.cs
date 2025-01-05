using AppCore.Runtime;
using R3;
using VContainer;

namespace App.InGame.Presentation.HUD
{
    public class HackingGamePresenter : IPresenter
    {
        private readonly CancellationDisposable cancellationDisposable;

        [Inject]
        public HackingGamePresenter(IDirector director, HackingGameApplicationService applicationService, InGameHudView inGameHudView,
            HackingGameView hackingGameView)
        {
            Director = director;
            ApplicationService = applicationService;
            InGameHudView = inGameHudView;
            HackingGameView = hackingGameView;
            cancellationDisposable = new CancellationDisposable();
            Setup();
        }
        private IDirector Director { get; set; }
        private InGameHudView InGameHudView { get; set; }
        private HackingGameView HackingGameView { get; set; }
        private HackingGameApplicationService ApplicationService { get; set; }

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
                .Subscribe(CheckPassword)
                .RegisterTo(cancellationDisposable.Token);

            HackingGameView.Request
                .Subscribe(text => ApplicationService.RequestPassword(text))
                .RegisterTo(cancellationDisposable.Token);

            ApplicationService.Setup();
            HackingGameView.Setup();
            InGameHudView.Open();
        }

        private void StartHacking(string passwordAnswer)
        {
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
    }
}