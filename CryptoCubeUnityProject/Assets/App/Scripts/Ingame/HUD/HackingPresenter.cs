using AppCore.Runtime;
using R3;
using VContainer;

namespace App.InGame.HUD
{
    public class HackingPresenter : IPresenter
    {
        private IDirector Director { get; set; }
        private InGameHudView InGameHudView { get; set; }
        private HackingTaskView HackingTaskView { get; set; }
        private HackingTaskModel Model { get; set; }
        private readonly CancellationDisposable cancellationDisposable;

        [Inject]
        public HackingPresenter(IDirector director, HackingTaskModel model, InGameHudView inGameHudView, HackingTaskView hackingTaskView)
        {
            Director = director;
            Model = model;
            InGameHudView = inGameHudView;
            HackingTaskView = hackingTaskView;
            cancellationDisposable = new CancellationDisposable();
            Setup();
        }

        public void Execute()
        {
            Model.Execute();
        }

        public void Dispose()
        {
            Director = null;
            InGameHudView.Pop();
            InGameHudView = null;
            HackingTaskView.Pop();
            HackingTaskView = null;
            Model.Dispose();
            Model = null;
            cancellationDisposable.Dispose();
        }

        private void Setup()
        {
            HackingTaskView.Push();
            HackingTaskView.Hide();
            
            InGameHudView.Push();
            InGameHudView.Hide();

            Model.HackingRemainTime
                .Subscribe(InGameHudView.SetRemainingTimeText)
                .RegisterTo(cancellationDisposable.Token);
            
            Model.StartHacking
                .Subscribe(StartHacking)
                .RegisterTo(cancellationDisposable.Token);

            Model.HackingRemainTime
                .Where(x => x == 0)
                .Subscribe(_ => EndState())
                .RegisterTo(cancellationDisposable.Token);

            Model.CheckPassword
                .Subscribe(CheckPassword)
                .RegisterTo(cancellationDisposable.Token);
            
            HackingTaskView.Request
                .Subscribe(text => Model.RequestPassword(text))
                .RegisterTo(cancellationDisposable.Token);
            
            Model.Setup();
            HackingTaskView.Setup();
            InGameHudView.Open();
        }

        private void StartHacking(string passwordAnswer)
        {
            HackingTaskView.PreOpenSetup(passwordAnswer);
            HackingTaskView.Open();   
        }
        
        private void CheckPassword(bool passwordCorrect)
        {
            if (!passwordCorrect) return;

            HackingTaskView.Hide();
            Model.UnlockSuccess();
        }

        private void EndState()
        {
            Model.ChangeEndState();
            InGameHudView.Close();
        }
    }
}