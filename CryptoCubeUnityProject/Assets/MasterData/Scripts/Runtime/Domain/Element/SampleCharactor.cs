using System;
using UnityEngine;

namespace MasterData.Runtime.Domain
{
    [Serializable]
    public class SampleCharacter
    {
        [SerializeField] private int id;
        [SerializeField] private string name;
        [SerializeField] private int type;
        
        public int Id => id;
        public string Name => name;
        public int Type => type;
    }
}
