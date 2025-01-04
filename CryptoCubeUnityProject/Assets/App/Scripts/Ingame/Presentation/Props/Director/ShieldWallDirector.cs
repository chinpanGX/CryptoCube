using System;
using System.Collections.Generic;
using App.InGame.Application;
using R3;
using UnityEngine;
using VContainer;

namespace App.InGame.Presentation.Props
{
    public class ShieldWallDirector : MonoBehaviour
    {
        [SerializeField] private List<ShieldWall> shieldWalls;

        private readonly Dictionary<int, ShieldWall> shieldWallDictionary = new();
        private ShieldWallApplicationService ApplicationService { get; set; }

        [Inject]
        public void Construct(ShieldWallApplicationService applicationService)
        {
            ApplicationService = applicationService;
            shieldWallDictionary.Clear();
            foreach (var shieldWall in shieldWalls)
            {
                shieldWall.Construct(applicationService.OnTriggerEnterPublish);
                shieldWallDictionary[shieldWall.Id] = shieldWall;
            }

            ApplicationService.OnUnlock
                .Subscribe(id =>
                    {
                        if (shieldWallDictionary.TryGetValue(id, out var shieldWall))
                        {
                            shieldWall.Unlock();
                        }
                    }
                ).RegisterTo(destroyCancellationToken);
        }

        private void OnDestroy()
        {
            ApplicationService?.Dispose();
        }
    }
}