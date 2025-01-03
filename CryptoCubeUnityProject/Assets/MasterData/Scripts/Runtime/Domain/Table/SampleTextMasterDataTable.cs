using System.Collections.Generic;
using System.Linq;

namespace MasterData.Runtime.Domain
{
    public sealed class SampleTextMasterDataTable : IMasterDataTable<SampleText>
    {
        private readonly Dictionary<int, SampleText> table;
        private List<SampleText> list;
        
        public SampleTextMasterDataTable(IEnumerable<SampleText> masterData)
        {
            table = masterData.ToDictionary(x => x.Id, x => x);
        }

        public SampleText GetById(int id)
        {
            return table[id];
        }
        
        public List<SampleText> GetAll()
        {
            return list ??= table.Values.ToList();
        }
    }
}