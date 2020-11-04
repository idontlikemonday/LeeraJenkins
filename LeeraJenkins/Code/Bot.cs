using System.Collections.Generic;
using LeeraJenkins.Helpers;
using LeeraJenkins.Logic.Commands;

namespace LeeraJenkins.Code
{
    public class Bot : IBot
    {
        public List<ICommand> Commands { get; set; } = new List<ICommand>();
        public List<ICommand> SpecialCommands { get; set; } = new List<ICommand>();

        public void InitializeCommands()
        {
            Commands.AddRange(CommandHelper.GetCommandList());
            SpecialCommands.AddRange(CommandHelper.GetSpecialCommandList());
        }
    }
}
