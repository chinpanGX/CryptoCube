namespace App.User.Domain
{
    public interface IUserRepository
    {
        void Save(UserEntity user);
        UserEntity Load();   
    }
}