using System.Collections.Generic;
using App.InGame.Domain;
using App.InGame.Domain.Message;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace App.InGame.Application
{
    public class CharacterSpawnApplicationService : ISpawn
    {
        private readonly IPublisher<OnTriggerEnterWithCharacterSpawnerMessage>
            onTriggerEnterWithCharacterSpawnerPublisher;
        private readonly List<ICharacterSpawner> characterSpawners;

        [Inject]
        public CharacterSpawnApplicationService(
            IPublisher<OnTriggerEnterWithCharacterSpawnerMessage> onTriggerEnterWithCharacterSpawnerPublisher,
            List<ICharacterSpawner> characterSpawners)
        {
            this.onTriggerEnterWithCharacterSpawnerPublisher = onTriggerEnterWithCharacterSpawnerPublisher;
            this.characterSpawners = characterSpawners;
        }
        
        public void OnTriggerEnterPublish(int index)
        {
            onTriggerEnterWithCharacterSpawnerPublisher.Publish(new OnTriggerEnterWithCharacterSpawnerMessage());
            characterSpawners[index].Unlock();
        }
        
        public Vector3 GetSpawnPosition()
        {
            var spawner = characterSpawners.FindLast(spawner => spawner.Unlocked);
            return spawner.Position;
        }
    }
}