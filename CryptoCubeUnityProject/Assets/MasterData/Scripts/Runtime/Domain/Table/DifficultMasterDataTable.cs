using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MasterData.Runtime.Domain
{
    [Serializable]
    public class DifficultMaster : IMasterData
    {
        [SerializeField] private int id;
        [SerializeField] private int difficultyId;
        [SerializeField] private string displayTextKey;
        
        public int Id => id;
        public int DifficultyId => difficultyId;
        public string DisplayTextKey => displayTextKey;
    }
    
    public sealed class DifficultMasterDataTable : MasterDataTable
    {
        private readonly Dictionary<int, DifficultMaster> table;
        private List<DifficultMaster> list = new();
        
        public DifficultMasterDataTable(IEnumerable<DifficultMaster> masterData)
        {
            table = masterData.ToDictionary(x => x.Id, x => x);
        }
        
        public DifficultMaster GetById(int id)
        {
            return table[id];
        }
        
        public List<DifficultMaster> GetAll()
        {
            if (!list.Any())
            {
                list = table.Values.ToList();
            }
            return list;
        }
    }
}
