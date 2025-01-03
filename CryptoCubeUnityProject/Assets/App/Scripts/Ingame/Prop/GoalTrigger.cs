using App.InGame.Message;
using MessagePipe;
using R3;
using R3.Triggers;
using UnityEngine;
using VContainer;

namespace App.InGame.Prop
{
    public class GoalTrigger : MonoBehaviour
    {
        [Inject]
        public void Construct(IPublisher<PlayerControlPermissionMessage> playerControlPermissionPublisher,
            IPublisher<OnTriggerEnterWithGoalMessage> onTriggerEnterWithGoalPublisher)
        {
            this.OnTriggerEnterAsObservable()
                .Where(x => x.gameObject.CompareTag("Player"))
                .Subscribe(_ =>
                    {
                        Debug.Log("Goal");
                        playerControlPermissionPublisher.Publish(new PlayerControlPermissionMessage(false));
                        onTriggerEnterWithGoalPublisher.Publish(new OnTriggerEnterWithGoalMessage());
                    }
                )
                .RegisterTo(destroyCancellationToken);
        }
    }
}