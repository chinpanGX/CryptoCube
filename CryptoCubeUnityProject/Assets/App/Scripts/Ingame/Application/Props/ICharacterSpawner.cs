using UnityEngine;

namespace App.InGame.Application
{
    public interface ICharacterSpawner
    {
        Vector3 Position { get; }
        bool Unlocked { get; }
        void Unlock();
    }
}