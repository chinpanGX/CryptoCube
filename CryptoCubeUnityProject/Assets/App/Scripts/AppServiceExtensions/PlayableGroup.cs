using System;
using UnityEngine;
using UnityEngine.Playables;

namespace App.AppServiceExtensions
{
    [Serializable]
    public class PlayableGroup
    {
        [SerializeField] private string key;
        [SerializeField] private PlayableAsset playableAsset;

        public string Key => key;
        public PlayableAsset PlayableAsset => playableAsset;
    }
}