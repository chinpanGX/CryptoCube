using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MasterData.Runtime.Domain;

namespace MasterData.Runtime.Infrastructure
{
    public partial class MasterDataLoader
    {
        private readonly List<MasterDataTable> tables = new();

        public async UniTask<List<MasterDataTable>> LoadAll()
        {
            await UniTask.WhenAll(
                LoadAsyncByLocalizationMaster(),
                LoadAsyncByStageMaster(),
                LoadAsyncByCharacterMaster(),
                LoadAsyncByHackMaster()
            );
            return tables;
        }

        private async UniTask LoadAsyncByLocalizationMaster()
        {
            var data = await LoadAsync<LocalizationMaster>("LocalizationMaster");
            tables.Add(new LocalizationMasterDataTable(data));
        }

        private async UniTask LoadAsyncByStageMaster()
        {
            var data = await LoadAsync<StageMaster>("StageMaster");
            tables.Add(new StageMasterDataTable(data));
        }

        private async UniTask LoadAsyncByCharacterMaster()
        {
            var data = await LoadAsync<DifficultMaster>("DifficultMaster");
            tables.Add(new DifficultMasterDataTable(data));
        }

        private async UniTask LoadAsyncByHackMaster()
        {
            var data = await LoadAsync<HackMaster>("HackMaster");
            tables.Add(new HackMasterDataTable(data));
        }
    }
}