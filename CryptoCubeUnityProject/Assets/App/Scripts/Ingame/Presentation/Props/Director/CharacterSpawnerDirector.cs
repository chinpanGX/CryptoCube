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
        
        public List<ICharacterSpawner> CharacterSpawnerInterfaces => characterSpawners.Cast<ICharacterSpawner>().ToList();
        
        [Inject]
        public void Construct(
            CharacterSpawnApplicationService applicationService)
        { 
            foreach (var characterSpawner in characterSpawners)
            {
                characterSpawner.Construct(() =>
                    applicationService.OnTriggerEnterPublish(characterSpawners.IndexOf(characterSpawner))
                );
            }
        }
    }
}