using System.Threading;
using App.InGame.Message;
using Cysharp.Threading.Tasks;
using LitMotion;
using MessagePipe;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.Splines;
using VContainer;
using DelayType = LitMotion.DelayType;

namespace App.InGame.Prop
{
    public class PatrolEnemy : MonoBehaviour
    {
        [SerializeField] private SplineContainer splineContainer;
        [SerializeField] private Transform actorTransform;
        [SerializeField] private Transform trigger;
        [SerializeField] private float speed = 5.0f;
        [SerializeField] private float cooltime = 1.0f;
        
        private MotionHandle handle;

        [Inject]
        public void Construct(IPublisher<OnTriggerEnterWithPatrolEnemyMessage> onTriggerEnterWithPatrolEnemyPublisher)
        {
            if (splineContainer == null)
            {
                Debug.LogError("SplineContainer is not set.");
                return;
            }
            
            if (actorTransform == null)
            {
                Debug.LogError("ActorTransform is not set.");
                return;
            }
            
            trigger.OnTriggerEnterAsObservable()
                .Where(x => x.gameObject.CompareTag("Player"))
                .SubscribeAwait(async (_, ct) =>
                    {
                        onTriggerEnterWithPatrolEnemyPublisher.Publish(new OnTriggerEnterWithPatrolEnemyMessage());
                        handle.Complete();
                        await UniTask.Delay((int)(cooltime * 100), cancellationToken: ct);
                        handle.Preserve();
                    }
                )
                .RegisterTo(destroyCancellationToken);
            
            // TODO　リスタート時
            // handle.Preserve();
            
            handle = LMotion.Create(0.0f, 1.0f, speed)
                .WithEase(Ease.Linear)
                .WithDelay(1.0f, DelayType.EveryLoop)
                .WithLoops(-1, LoopType.Yoyo)
                .Bind(BindToMotion)
                .AddTo(gameObject);

            handle.Preserve();
        }
        
        private void BindToMotion(float value)
        {
            splineContainer.Spline.Evaluate(value, out var splinePos, out var splineTangent, out var upVector);
            transform.SetPositionAndRotation(new Vector3(splinePos.x, transform.position.y, splinePos.z),
                Quaternion.LookRotation(splineTangent, upVector)
            );
        }
    }

}