using System.Collections.Generic;

namespace MasterData.Runtime.Domain
{
    public interface IMasterDataTable<T>
    {
        T GetById(int id);
        List<T> GetAll();   
    }
}