using System.Collections.Generic;
using LeeraJenkins.Logic.Commands;

namespace LeeraJenkins.Code
{
    public interface IBot
    {
        List<ICommand> Commands { get; set; }
        List<ICommand> SpecialCommands { get; set; }
        void InitializeCommands();
    }
}
