using System.Collections.Generic;
using System.Linq;

namespace MasterData.Runtime.Domain
{
    public sealed class SampleCharacterMasterDataTable : IMasterDataTable<SampleCharacter>
    {
        private readonly Dictionary<int, SampleCharacter> table;
        private List<SampleCharacter> list;
        
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
            return list ??= table.Values.ToList();
        }
    }

}