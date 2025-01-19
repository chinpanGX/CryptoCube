using System;
using MasterData.Runtime.Domain;
using UnityEngine;

namespace App.User.Domain
{
    [Serializable]
    public class UserEntity
    {
        [SerializeField] private Language language;
        [SerializeField] private int clearStageCount;
        
        public Language Language => language;
        public int ClearStageCount => clearStageCount;
        
        public static UserEntity CreateDefault()
        {
            return new UserEntity(Language.Japanese, 0);
        }
        
        private UserEntity(Language language, int clearStageCount)
        {
            this.language = language;
            this.clearStageCount = clearStageCount;
        }
        
        public UserEntity ChangeLanguage(Language language)
        {
            return new UserEntity(language, clearStageCount);
        }
        
        public UserEntity AddClearStageCount(int addCount)
        {
            return new UserEntity(language, clearStageCount + addCount);
        }
    }
}