using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using MasterData.Runtime.Domain;

namespace MasterData.Runtime.Infrastructure
{
    public class MasterDataRepository : IMasterRepository
    {
        private List<MasterDataTable> tables;

        public T GetTable<T>() where T : class
        {
            var master = tables.FirstOrDefault(x => x is T);
            if (master == null)
            {
                throw new MasterDataException($"Master data table not found: {typeof(T).Name}");
            }
            return master as T;
        }

        public async UniTask LoadAsync()
        {
            var loader = new MasterDataLoader();
            tables = await loader.LoadAll();
        }
    }
}