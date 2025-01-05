using System;
using R3;
using R3.Triggers;
using UnityEngine;

namespace App.InGame.Presentation.Props
{
    public class ShieldWall : MonoBehaviour
    {
        [SerializeField] private int id;
        public int Id => id;
        
        public void Construct(Action<int> onTrigger)
        {
            this.OnTriggerEnterAsObservable()
                .Where(x => x.gameObject.CompareTag("Player"))
                .Subscribe(_ =>
                    {
                        onTrigger.Invoke(id);
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