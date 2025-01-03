using AppCore.Runtime;
using Cysharp.Threading.Tasks;
using R3;
using UnityEditor;
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
                .Subscribe(x => InGameHudView.SetRemainingTimeText($"{x}"))
                .AddTo(cancellationDisposable.Token);
            
            Model.StartHacking
                .Subscribe(text =>
                    {
                        HackingTaskView.PreOpenSetup(text);
                        HackingTaskView.Open();
                    }
                )
                .AddTo(cancellationDisposable.Token);

            Model.HackingRemainTime
                .Where(x => x == 0)
                .Subscribe(_ => EndState())
                .AddTo(cancellationDisposable.Token);
            
            Model.Setup();
            InGameHudView.Open();
        }

        private void EndState()
        {
            Model.ChangeEndState();
            InGameHudView.Close();
        }
    }
}