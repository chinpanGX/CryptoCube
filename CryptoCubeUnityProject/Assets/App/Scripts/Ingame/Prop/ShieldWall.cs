using App.InGame.Message;
using MessagePipe;
using R3;
using R3.Triggers;
using UnityEngine;
using VContainer;

namespace App.InGame.Prop
{
    public class ShieldWall : MonoBehaviour
    {
        [SerializeField] private int id;
        public int Id => id;

        [Inject]
        public void Construct(
            IPublisher<OnTriggerEnterWithShieldWallMessage> onTriggerEnterWithShieldWallPublisher)
        {
            this.OnTriggerEnterAsObservable()
                .Where(x => x.gameObject.CompareTag("Player"))
                .Subscribe(_ =>
                    {
                        onTriggerEnterWithShieldWallPublisher.Publish(new OnTriggerEnterWithShieldWallMessage(id));
                    }
                )
                .RegisterTo(destroyCancellationToken);
        }

        public void Unlock()
        {
            Destroy(gameObject);
        }
    }
}