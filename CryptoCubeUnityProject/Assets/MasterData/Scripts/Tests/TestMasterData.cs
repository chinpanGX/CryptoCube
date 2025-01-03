using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using MasterData.Runtime.Domain;
using MasterData.Runtime.Infrastructure;

namespace MasterData.Tests
{
    public class TestMasterData
    {
        [Test]
        public async Task TestMasterDataLoader()
        {
            var loader = new MasterDataLoader();
            var asset = await loader.LoadAsync<SampleText>("MasterData/Resource/SampleText.json");
            var sampleTextMasterDataTable = new SampleTextMasterDataTable(asset);
            Assert.AreEqual(3, sampleTextMasterDataTable.GetAll().Count);
            
            var asset2 = await loader.LoadAsync<SampleCharacter>("MasterData/Resource/SampleCharacter.json");
            var sampleCharacterMasterDataTable = new SampleCharacterMasterDataTable(asset2);
            Assert.AreEqual(3, sampleCharacterMasterDataTable.GetAll().Count);
            Assert.AreEqual("ピカチュウ", sampleCharacterMasterDataTable.GetById(1).Name);
        } 
    }
}
