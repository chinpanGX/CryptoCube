using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MasterData.Runtime.Domain
{
    [Serializable]
    public class SampleText : IMasterData
    {
        [SerializeField] private int id;
        [SerializeField] private string text;

        public int Id => id;
        public string Text => text;
    }
    
    public sealed class SampleTextMasterDataTable : MasterDataTable
    {
        private readonly Dictionary<int, SampleText> table;
        private List<SampleText> list = new();
        
        public SampleTextMasterDataTable(SampleText[] data)
        {  
            table = data.ToDictionary(x => x.Id, x => x);
        }
        
        public SampleText GetById(int id)
        {
            return table[id];
        }
        
        public List<SampleText> GetAll()
        {
            if (!list.Any())
            {
                list = table.Values.ToList();
            }
            return list;
        }
    }
}