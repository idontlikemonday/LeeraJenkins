using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeeraJenkins.DbRepository.User
{
    using User = LeeraJenkins.Db.User;

    public interface IUserRepository
    {
        Task RegisterUser(User user);
        Task<bool> CheckRegistration(User user);
        Task<List<User>> GetAllUsers();
        Task<long?> GetChatId(string tgName);
        Task<long?> GetUserId(string tgName);
        Task<long?> GetUserId(long chatId);
        Task<User> GetUser(string tgName);
        Task<User> GetUser(long userId);
    }
}
