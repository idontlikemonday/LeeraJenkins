using LeeraJenkins.Db;

namespace LeeraJenkins.DbRepository.Logger
{
    public interface IUserActionLoggerRepository
    {
        void AddUserAction(UserAction userAction);
    }
}
