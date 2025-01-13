using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MasterData.Runtime.Domain
{
    [Serializable]
    public class HackMaster : IMasterData
    {
        [SerializeField] private int id;
        [SerializeField] private string missionText;

        public int Id => id;
        public string MissionText => missionText;
    }
    
    public class HackMasterDataTable : MasterDataTable
    {
        private readonly Dictionary<int, HackMaster> table;
        private List<HackMaster> list = new();
        
        public HackMasterDataTable(IEnumerable<HackMaster> masterData)
        {
            table = masterData.ToDictionary(x => x.Id, x => x);
        }
        
        public HackMaster GetById(int id)
        {
            return table[id];
        }
        
        public List<HackMaster> GetAll()
        {
            if (!list.Any())
            {
                list = table.Values.ToList();
            }
            return list;
        }
    }
}
