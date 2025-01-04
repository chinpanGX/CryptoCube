using App.InGame.Application;
using R3;
using R3.Triggers;
using UnityEngine;
using VContainer;

namespace App.InGame.Presentation.Props
{
    public class GoalTrigger : MonoBehaviour
    {
        [Inject]
        public void Construct(GoalApplicationService goalApplicationService)
        {
            this.OnTriggerEnterAsObservable()
                .Where(x => x.gameObject.CompareTag("Player"))
                .Subscribe(_ =>
                    {
                        goalApplicationService.OnTriggerEnterPublish();
                    }
                )
                .RegisterTo(destroyCancellationToken);
        }
    }
}