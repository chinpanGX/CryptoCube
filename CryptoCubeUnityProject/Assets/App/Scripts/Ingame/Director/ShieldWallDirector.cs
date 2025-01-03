using System.Collections.Generic;
using System.Linq;
using App.InGame.Message;
using App.InGame.Prop;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace App.InGame.HUD
{
    public class ShieldWallDirector : MonoBehaviour
    {
        [SerializeField] private List<ShieldWall> shieldWalls;

        private readonly Dictionary<int, ShieldWall> shieldWallDictionary = new();
        
        [Inject]
        public void Construct(
            IPublisher<OnTriggerEnterWithShieldWallMessage> onTriggerEnterWithShieldWallPublisher,
            IPublisher<PlayerControlPermissionMessage> playerControlPermissionPublisher,
            ISubscriber<UnlockedShieldWallMessage> unlockedShieldWallSubscriber)
        {
            shieldWallDictionary.Clear();
            foreach (var shieldWall in shieldWalls)
            {
                shieldWall.Construct(onTriggerEnterWithShieldWallPublisher);
                shieldWallDictionary[shieldWall.Id] = shieldWall;
            }
            
            unlockedShieldWallSubscriber.Subscribe(message =>
            {
                if (shieldWallDictionary.TryGetValue(message.ShieldWallId, out var shieldWall))
                {
                    shieldWall.Unlock();
                }
            });
        }
    }
}