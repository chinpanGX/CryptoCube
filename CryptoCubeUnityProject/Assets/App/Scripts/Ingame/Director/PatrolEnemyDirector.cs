using System.Collections.Generic;
using App.InGame.Message;
using App.InGame.Prop;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace App.InGame.HUD
{
    public class PatrolEnemyDirector : MonoBehaviour
    {
        [SerializeField] private List<PatrolEnemy> patrolEnemies;
        
        [Inject]
        public void Construct(IPublisher<OnTriggerEnterWithPatrolEnemyMessage> onTriggerEnterWithPatrolEnemyPublisher)
        {
            foreach (var patrolEnemy in patrolEnemies)
            {
                patrolEnemy.Construct(onTriggerEnterWithPatrolEnemyPublisher);
            }
        }
    }
}