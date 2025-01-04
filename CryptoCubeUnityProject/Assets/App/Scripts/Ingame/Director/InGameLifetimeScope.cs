using App.InGame.Player;
using App.InGame.Prop;
using AppCore.Runtime;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace App.InGame.HUD
{
    public class InGameLifetimeScope : LifetimeScope
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private HackingTaskView hackingTaskView;
        [SerializeField] private InGameHudView inGameHudView;
        [SerializeField] private GoalTrigger goalTrigger;
        [SerializeField] private ShieldWallDirector shieldWallDirector;
        [SerializeField] private PatrolEnemyDirector patrolEnemyDirector;
        [SerializeField] private CharacterSpawner characterSpawner;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterMessagePipe();
            builder.RegisterComponent(characterSpawner).AsImplementedInterfaces();
            builder.RegisterComponent(patrolEnemyDirector);
            builder.RegisterComponent(goalTrigger);
            builder.RegisterComponent(playerController);
            builder.RegisterComponent(hackingTaskView);
            builder.RegisterComponent(inGameHudView);
            builder.RegisterComponent(shieldWallDirector);
            builder.Register<UpdatablePresenter>(Lifetime.Scoped);
            builder.Register<HackingTaskModel>(Lifetime.Scoped);
            builder.Register<HackingPresenter>(Lifetime.Scoped);
            builder.RegisterEntryPoint<InGameDirector>();
        }

        private class InGameDirector : IDirector, IStartable, ITickable
        {
            private readonly InGameLifetimeScope lifetimeScope;
            private readonly UpdatablePresenter updatablePresenter;

            [Inject]
            public InGameDirector(InGameLifetimeScope lifetimeScope, UpdatablePresenter updatablePresenter)
            {
                this.lifetimeScope = lifetimeScope;
                this.updatablePresenter = updatablePresenter;
            }

            public void Push(string name)
            {
                switch (name)
                {
                    case "InGame":
                        var request = lifetimeScope.Container.Resolve<HackingPresenter>();
                        updatablePresenter.SetRequest(request);
                        break;
                }
            }

            public void Start()
            {
                Push("InGame");
            }

            public void Tick()
            {
                updatablePresenter.Execute();
            }
        }
    }
}