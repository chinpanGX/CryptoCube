using System;
using System.Threading;
using R3;
using UnityEngine;

namespace App.InGame.Presentation.Props
{
    internal class TrapLine : MonoBehaviour
    {
        [SerializeField] private Transform trigger;

        public void Construct(Action action)
        {
            trigger.OnTriggerEnterToPlayer()
                .Subscribe(_ => action.Invoke())
                .RegisterTo(destroyCancellationToken);
        }
    }
}