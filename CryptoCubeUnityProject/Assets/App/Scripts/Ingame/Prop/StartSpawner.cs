using App.InGame.Message;
using MessagePipe;
using R3;
using R3.Triggers;
using UnityEngine;
using VContainer;

namespace App.InGame.Prop
{
    public class CharacterSpawner : MonoBehaviour, ICharacterSpawner
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private bool started;
        
        public Vector3 Position => transform.position;
        public bool Unlocked { get; private set; }
        
        private IPublisher<OnTriggerEnterWithCharacterSpawnerMessage> OnTriggerEnterWithCharacterSpawnerPublisher { get; set; }

        [Inject]
        public void Construct(
            IPublisher<OnTriggerEnterWithCharacterSpawnerMessage> onTriggerEnterWithCharacterSpawnerPublisher)
        {
            if (started)
                Unlocked = true;
            Unlocked = false;

            OnTriggerEnterWithCharacterSpawnerPublisher = onTriggerEnterWithCharacterSpawnerPublisher;
            this.OnTriggerEnterAsObservable()
                .Where(x => x.gameObject.CompareTag("Player") && !Unlocked)
                .Subscribe(_ => Unlock())
                .AddTo(this);
        }

        private void Unlock()
        {
            OnTriggerEnterWithCharacterSpawnerPublisher.Publish(
                new OnTriggerEnterWithCharacterSpawnerMessage()
            );
            Unlocked = true;
        }
        
        void Awake()
        {
            meshRenderer.enabled = false;
        }
    }
}