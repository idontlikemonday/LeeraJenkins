using LeeraJenkins.Db;

namespace LeeraJenkins.DbRepository.Logger
{
    public class UserActionLoggerRepository : IUserActionLoggerRepository
    {
        public void AddUserAction(UserAction userAction)
        {
            using (var context = new TgEntities())
            {
                var userActions = context.UserAction;
                userActions.Add(userAction);

                context.SaveChanges();
            }
        }
    }
}
