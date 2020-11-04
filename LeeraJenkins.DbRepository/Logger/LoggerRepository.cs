using LeeraJenkins.Db;

namespace LeeraJenkins.DbRepository.Logger
{
    public class LoggerRepository : ILoggerRepository
    {
        public void AddLog(Log log)
        {
            using (var context = new TgEntities())
            {
                var logs = context.Log;
                logs.Add(log);

                context.SaveChanges();
            }
        }
    }
}
