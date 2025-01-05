using System;
using System.Collections.Generic;
using System.Linq;
using App.InGame.Application;
using UnityEngine;
using VContainer;

namespace App.InGame.Presentation.Props
{
    public class CharacterSpawnerDirector : MonoBehaviour
    {
        [SerializeField] private List<CharacterSpawner> characterSpawners;

        public List<ICharacterSpawner> CharacterSpawnerInterfaces =>
            characterSpawners.Cast<ICharacterSpawner>().ToList();

        [Inject]
        public void Construct(
            CharacterSpawnApplicationService applicationService)
        {
            Validate();
            
            foreach (var characterSpawner in characterSpawners)
            {
                characterSpawner.Construct(() =>
                    applicationService.OnTriggerEnterPublish(characterSpawners.IndexOf(characterSpawner))
                );
            }
        }

        private void Validate()
        {
            if (!characterSpawners[0].Unlocked)
                throw new Exception("The first character spawner must be unlocked.");
            if (characterSpawners.Skip(1).Any(spawner => spawner.Unlocked))
                throw new Exception("The second and subsequent character spawners must be locked.");
        }
    }
}