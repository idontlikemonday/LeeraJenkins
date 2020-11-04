using System.Threading.Tasks;

namespace LeeraJenkins.Logic.User
{
    using LeeraJenkins.Model.Core;

    public interface IUserLogic
    {
        Task<long?> GetUserId(string tgName);
        Task<long?> GetUserId(long chatId);
        Task<User> GetUser(string tgName);
        Task<User> GetUser(long userId);
        Task GetPhoto();
    }
}
