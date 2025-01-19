using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MasterData.Runtime.Domain
{
    [Serializable]
    public class SampleCharacter : IMasterData
    {
        [SerializeField] private int id;
        [SerializeField] private string name;
        [SerializeField] private int type;
        
        public int Id => id;
        public string Name => name;
        public int Type => type;
    }
    
    public sealed class SampleCharacterMasterDataTable : MasterDataTable
    {
        private readonly Dictionary<int, SampleCharacter> table = new ();
        private List<SampleCharacter> list = new ();

        public SampleCharacterMasterDataTable(IEnumerable<SampleCharacter> masterData)
        {
            table = masterData.ToDictionary(x => x.Id, x => x);
        }

        public SampleCharacter GetById(int id)
        {
            return table[id];
        }

        public List<SampleCharacter> GetAll()
        {
            if (!list.Any())
            {
                list = table.Values.ToList();
            }
            return list;
        }
    }
}