using System;

namespace LeeraJenkins.Model.Logger
{
    public class Log
    {
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}
