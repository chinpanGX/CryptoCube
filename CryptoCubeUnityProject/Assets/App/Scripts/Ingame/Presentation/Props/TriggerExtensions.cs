using R3;
using R3.Triggers;
using UnityEngine;

namespace App.InGame.Presentation.Props
{
    internal static class TriggerExtensions
    {
        public static Observable<Collider> OnTriggerEnterToPlayer(this Transform transform)
        {
            return transform.OnTriggerEnterAsObservable()
                .Where(x => x.gameObject.CompareTag("Player"));
        }
    }
}