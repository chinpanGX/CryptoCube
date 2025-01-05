using AppCore.Runtime;
using R3;
using VContainer;

namespace App.InGame.Presentation.HUD
{
    public class HackingGamePresenter : IPresenter
    {
        private readonly CancellationDisposable cancellationDisposable;

        [Inject]
        public HackingGamePresenter(IDirector director, HackingGameUseCase useCase, InGameHudView inGameHudView,
            HackingGameView hackingGameView)
        {
            Director = director;
            UseCase = useCase;
            InGameHudView = inGameHudView;
            HackingGameView = hackingGameView;
            cancellationDisposable = new CancellationDisposable();
            Setup();
        }
        private IDirector Director { get; set; }
        private InGameHudView InGameHudView { get; set; }
        private HackingGameView HackingGameView { get; set; }
        private HackingGameUseCase UseCase { get; set; }

        public void Execute()
        {
            UseCase.Execute();
        }

        public void Dispose()
        {
            Director = null;
            InGameHudView.Pop();
            InGameHudView = null;
            HackingGameView.Pop();
            HackingGameView = null;
            UseCase.Dispose();
            UseCase = null;
            cancellationDisposable.Dispose();
        }

        private void Setup()
        {
            HackingGameView.Push();
            HackingGameView.Hide();

            InGameHudView.Push();
            InGameHudView.Hide();

            UseCase.HackingRemainTime
                .Subscribe(InGameHudView.SetRemainingTimeText)
                .RegisterTo(cancellationDisposable.Token);

            UseCase.StartHacking
                .Subscribe(StartHacking)
                .RegisterTo(cancellationDisposable.Token);

            UseCase.HackingRemainTime
                .Where(x => x == 0)
                .Subscribe(_ => EndState())
                .RegisterTo(cancellationDisposable.Token);

            UseCase.CheckPassword
                .Subscribe(CheckPassword)
                .RegisterTo(cancellationDisposable.Token);

            HackingGameView.Request
                .Subscribe(text => UseCase.RequestPassword(text))
                .RegisterTo(cancellationDisposable.Token);

            UseCase.Setup();
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
            UseCase.UnlockSuccess();
        }

        private void EndState()
        {
            UseCase.ChangeEndState();
            InGameHudView.Close();
        }
    }
}