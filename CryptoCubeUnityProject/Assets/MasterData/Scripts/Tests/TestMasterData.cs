using System.Collections;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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
            var repository = new MasterDataRepository();
            await repository.LoadAsync();
            var sampleTextMasterDataTable = repository.GetTable<SampleTextMasterDataTable>();
            Assert.AreEqual(3, sampleTextMasterDataTable.GetAll().Count);
            
            var sampleCharacterMasterDataTable = repository.GetTable<SampleCharacterMasterDataTable>();
            Assert.AreEqual(3, sampleCharacterMasterDataTable.GetAll().Count);
            Assert.AreEqual("ピカチュウ", sampleCharacterMasterDataTable.GetById(1).Name);
        } 
    }
}
