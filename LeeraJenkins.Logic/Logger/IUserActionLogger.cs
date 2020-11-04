namespace LeeraJenkins.Logic.Logger
{
    public interface IUserActionLogger
    {
        void AddUserAction(string username, string action);
    }
}
