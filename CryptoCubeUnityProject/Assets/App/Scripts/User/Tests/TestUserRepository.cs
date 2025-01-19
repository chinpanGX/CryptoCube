using System.Collections;
using App.User.Infrastructure;
using MasterData.Runtime.Domain;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace App.User.Tests
{
    public class TestUserRepository
    {
        [Test, Description("ユーザーデータを作成する")]
        public void CreateUserData()
        {
            var userRepository = new UserRepository();
            var userEntity = userRepository.Load();
            userEntity = userEntity.ChangeLanguage(Language.Japanese);
            userEntity = userEntity.AddClearStageCount(0);
            userRepository.Save(userEntity);
            
            Assert.AreEqual(Language.Japanese, userEntity.Language);
            Assert.AreEqual(0, userEntity.ClearStageCount);
            userRepository.Save(userEntity);
        }
    }
}
