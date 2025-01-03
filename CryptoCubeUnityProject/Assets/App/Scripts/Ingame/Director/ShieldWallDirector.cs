using System.Collections.Generic;
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
        
        [Inject]
        public void Construct(
            IPublisher<OnTriggerEnterWithShieldWallMessage> onTriggerEnterWithShieldWallPublisher,
            IPublisher<PlayerControlPermissionMessage> playerControlPermissionPublisher)
        {
            foreach (var shieldWall in shieldWalls)
            {
                shieldWall.Construct(onTriggerEnterWithShieldWallPublisher, playerControlPermissionPublisher);
            }
        }
    }
}