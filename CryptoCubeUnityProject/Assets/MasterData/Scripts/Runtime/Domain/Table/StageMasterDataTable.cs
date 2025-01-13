using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MasterData.Runtime.Domain
{
    [Serializable]
    public class StageMaster : IMasterData
    {
        [SerializeField] private int id;
        [SerializeField] private string difficultyId;
        [SerializeField] private int limitSecondTime;
        
        public int Id => id;
        public string DifficultyId => difficultyId;
        public int LimitSecondTime => limitSecondTime;
    }

    public sealed class StageMasterDataTable : MasterDataTable
    {
        private readonly Dictionary<int, StageMaster> table = new();
        private List<StageMaster> list = new();
        
        public StageMasterDataTable(IEnumerable<StageMaster> masterData)
        {
            table = masterData.ToDictionary(x => x.Id, x => x);
        }
        
        public StageMaster GetById(int id)
        {
            return table[id];
        }

        public List<StageMaster> GetAll()
        {
            if (!list.Any())
            {
                list = table.Values.ToList();
            }
            return list;
        }
    }
}
