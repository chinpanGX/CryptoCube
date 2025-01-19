using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MasterData.Runtime.Domain
{
    [Serializable]
    public class LocalizationMaster : IMasterData
    {
        [SerializeField] private int id;
        [SerializeField] private string key;
        [SerializeField] private string jp;
        [SerializeField] private string en;

        public int Id => id;
        public string Key => key;
        public string Jp => jp;
        public string En => en;
    }

    public sealed class LocalizationMasterDataTable : MasterDataTable
    {
        private readonly Dictionary<string, LocalizationMaster> table;

        public LocalizationMasterDataTable(IEnumerable<LocalizationMaster> masterData)
        {
            table = masterData.ToDictionary(x => x.Key, x => x);
        }

        public LocalizationMaster GetByKey(string key)
        {
            return table[key];
        }
    }
}