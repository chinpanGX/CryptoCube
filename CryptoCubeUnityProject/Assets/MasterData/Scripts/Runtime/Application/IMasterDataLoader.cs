using System;
using Cysharp.Threading.Tasks;

namespace MasterData.Runtime.Application
{
    public interface IMasterDataLoader
    {
        UniTask<T[]> LoadAsync<T>(string assetName);
    }
    
    
    public class MasterDataException : Exception
    {
        public MasterDataException(string message) : base(message) { }
    }
}