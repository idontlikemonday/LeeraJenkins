using LeeraJenkins.Db;

namespace LeeraJenkins.DbRepository.Logger
{
    public interface ILoggerRepository
    {
        void AddLog(Log log);
    }
}
