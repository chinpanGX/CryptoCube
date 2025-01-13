using System;
using App.InGame.Application;
using R3;
using R3.Triggers;
using UnityEngine;

namespace App.InGame.Presentation.Props
{
    internal class CharacterSpawner : MonoBehaviour, ICharacterSpawner
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private bool started;

        public Vector3 Position => transform.position;

        private bool unlocked;
        public bool Unlocked => unlocked || started;

        public void Construct(Action onTriggerEnterAction)
        {
            this.OnTriggerEnterAsObservable()
                .Where(x => x.gameObject.CompareTag("Player") && !Unlocked)
                .Subscribe(_ => onTriggerEnterAction.Invoke())
                .AddTo(this);
        }

        public void Unlock()
        {
            unlocked = true;
        }

        void Awake()
        {
            meshRenderer.enabled = false;
        }
    }
}