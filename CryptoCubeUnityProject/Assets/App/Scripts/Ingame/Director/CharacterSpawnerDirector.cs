using System.Collections.Generic;
using System.Linq;
using App.InGame.Message;
using App.InGame.Prop;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace App.InGame.HUD
{
    public class CharacterSpawnerDirector : MonoBehaviour
    {
        [SerializeField] private List<CharacterSpawner> characterSpawners;
        
        [Inject]
        public void Construct(
            IPublisher<OnTriggerEnterWithCharacterSpawnerMessage> onTriggerEnterWithCharacterSpawnerPublisher)
        { 
            foreach (var characterSpawner in characterSpawners)
            {
                characterSpawner.Construct(onTriggerEnterWithCharacterSpawnerPublisher);
            }
        }

        private ICharacterSpawner GetLastUnlockedCharacterSpawner()
        {
            return characterSpawners.Last(spawner => spawner.Unlocked);
        }
    }
}