using System.Threading.Tasks;
using LeeraJenkins.Model.Dialog;
using LeeraJenkins.Model.Enum;

namespace LeeraJenkins.Logic.NewGame
{
    public interface INewGameLogic
    {
        Task<DialogDispatcherResult> ValidateAndAddNewStepWithValue(UserDialog userDialog, string value);
        Task<DialogDispatcherResult> GoToNextStep(UserDialog userDialog, Model.Core.Player initiator);
        Task<DialogDispatcherResult> FinalizeDialog(long userDialogId, Model.Core.Player initiator);
    }
}
