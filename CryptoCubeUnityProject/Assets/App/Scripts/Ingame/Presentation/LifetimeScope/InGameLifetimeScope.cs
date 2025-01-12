using App.InGame.Application;
using App.InGame.Domain;
using App.InGame.Presentation.HUD;
using App.InGame.Presentation.Player;
using App.InGame.Presentation.Props;
using AppCore.Runtime;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace App.InGame.Presentation
{
    public class InGameLifetimeScope : LifetimeScope
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private HackingGameView hackingGameView;
        [SerializeField] private InGameHudView inGameHudView;
        [SerializeField] private GoalTrigger goalTrigger;
        [SerializeField] private ShieldWallDirector shieldWallDirector;
        [SerializeField] private PatrolEnemyDirector patrolEnemyDirector;
        [SerializeField] private TrapLineDirector trapLineDirector;
        [SerializeField] private CharacterSpawnerDirector characterSpawnerDirector;
        [SerializeField] private CameraGroup cameraGroup;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterMessagePipe();
            builder.RegisterInstance(cameraGroup).AsImplementedInterfaces();
            builder.RegisterInstance(characterSpawnerDirector.CharacterSpawnerInterfaces);
            builder.RegisterComponent(characterSpawnerDirector);
            builder.RegisterComponent(patrolEnemyDirector);
            builder.RegisterComponent(trapLineDirector); 
            builder.RegisterComponent(goalTrigger);
            builder.RegisterComponent(playerController);
            builder.RegisterComponent(hackingGameView);
            builder.RegisterComponent(inGameHudView);
            builder.RegisterComponent(shieldWallDirector);
            builder.Register<PlayerApplicationService>(Lifetime.Scoped);
            builder.Register<CharacterSpawnApplicationService>(Lifetime.Scoped).As<ISpawn>().AsSelf();
            builder.Register<GoalApplicationService>(Lifetime.Scoped);
            builder.Register<ShieldWallApplicationService>(Lifetime.Scoped);
            builder.Register<PatrolEnemyApplicationService>(Lifetime.Scoped);
            builder.Register<TrapLineApplicationService>(Lifetime.Scoped);
            builder.Register<UpdatablePresenter>(Lifetime.Scoped);
            builder.Register<HackingGameApplicationService>(Lifetime.Scoped);
            builder.Register<HackingGamePresenter>(Lifetime.Scoped);
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
                        var request = lifetimeScope.Container.Resolve<HackingGamePresenter>();
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