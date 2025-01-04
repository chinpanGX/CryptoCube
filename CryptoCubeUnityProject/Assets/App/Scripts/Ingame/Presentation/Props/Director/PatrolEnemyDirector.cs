using System;
using System.Collections.Generic;
using App.InGame.Application;
using R3;
using UnityEngine;
using VContainer;

namespace App.InGame.Presentation.Props
{
    public class PatrolEnemyDirector : MonoBehaviour
    {
        [SerializeField] private List<PatrolEnemy> patrolEnemies;
        
        private PatrolEnemyApplicationService ApplicationService { get; set; }
        
        [Inject]
        public void Construct(PatrolEnemyApplicationService applicationService)
        {
            ApplicationService = applicationService;
            foreach (var patrolEnemy in patrolEnemies)
            {
                patrolEnemy.Construct(applicationService.OnTriggerEnterPublish);
            }
            
            Setup();
        }

        private void Setup()
        {
            ApplicationService.OnRestart.Subscribe(_ =>
                {
                    patrolEnemies.ForEach(x => x.PlayMotion());
                }
            ).AddTo(this);
        }

        private void OnDestroy()
        {
            ApplicationService?.Dispose();
        }
    }
}