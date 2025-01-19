using System.IO;
using App.User.Domain;
using UnityEngine;

namespace App.User.Infrastructure
{
    public class UserRepository : IUserRepository
    {
        private static readonly string DataPath = Application.persistentDataPath + "/user.json"; 
        
        public void Save(UserEntity userEntity)
        {
            var json = JsonUtility.ToJson(userEntity);
            File.WriteAllText(DataPath, json);
        }
        
        public UserEntity Load()
        {
            if (!File.Exists(DataPath))
            {
                return UserEntity.CreateDefault();
            }
            var json = File.ReadAllText(DataPath);
            return JsonUtility.FromJson<UserEntity>(json);
        }
    }
}
