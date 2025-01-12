using System;
using R3;
using UnityEngine;

namespace App.InGame.Presentation.Props
{
    internal class TrapLine : MonoBehaviour
    {
        public void Construct(Action action)
        {
            transform.OnTriggerEnterToPlayer()
                .Subscribe(_ => action.Invoke())
                .RegisterTo(destroyCancellationToken);
        }
    }
}