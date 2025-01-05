using System;
using App.InGame.Application;
using LitMotion;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.Splines;

namespace App.InGame.Presentation.Props
{
    internal class PatrolEnemy : MonoBehaviour
    {
        [SerializeField] private SplineContainer splineContainer;
        [SerializeField] private Transform actorTransform;
        [SerializeField] private Transform trigger;
        [SerializeField] private float speed = 5.0f;

        private MotionHandle handle;
        private PatrolEnemyApplicationService applicationService;
        private double motionTime = 0.0;

        public void Construct(Action onTriggerAction)
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
                .Subscribe(_ => OnTrigger(onTriggerAction)
                )
                .RegisterTo(destroyCancellationToken);
            
            PlayMotion();
        }

        public void PlayMotion()
        {
            handle = LMotion.Create(0.0f, 1.0f, speed)
                .WithEase(Ease.Linear)
                .WithLoops(-1, LoopType.Flip)
                .Bind(BindToMotion)
                .AddTo(gameObject);
            handle.Time = motionTime;
        }
        
        private void OnTrigger(Action onTrigger)
        {
            onTrigger.Invoke();
            motionTime = handle.Time;
            handle.TryCancel();
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