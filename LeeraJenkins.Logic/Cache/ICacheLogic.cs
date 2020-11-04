namespace LeeraJenkins.Logic.Cache
{
    public interface ICacheLogic
    {
        bool IsExists(string key);
        string Get(string key);
        void Set(string key, string obj);
    }
}
