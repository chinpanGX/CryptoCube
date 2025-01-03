using System;
using UnityEngine;

namespace MasterData.Runtime.Domain
{
    [Serializable]
    public class SampleText
    {
        [SerializeField] private int id;
        [SerializeField] private string text;
        
        public int Id => id;
        public string Text => text;
    }
}