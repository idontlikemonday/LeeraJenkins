using System;

namespace LeeraJenkins.Logic.Logger
{
    public interface ILogger
    {
        void LogError(string message, Exception ex);
    }
}
