using System.Collections.Generic;
using System.Threading.Tasks;
using LeeraJenkins.Model.Dialog;
using LeeraJenkins.Model.Enum;

namespace LeeraJenkins.Logic.Dialog
{
    public interface IDialogLogic
    {
        Task<UserDialog> InitiateDialog(UserDialog userDialog);
        Task AddStep(DialogStep step);
        Task<UserDialog> GetLastActiveDialog(long userId);

        Task<UserDialog> GetLastPausedDialog(long userId);
        Task<UserDialog> GetLastActiveOrPausedDialog(long userId);

        Task<DialogStep> GetLastDialogStep(long userDialogId);
        Task<DialogStep> GetLastDialogStepForUser(long userId);

        Task ContinueUserDialog(long userDialogId);
        Task PauseUserDialog(long userDialogId);
        Task FinalizeUserDialog(long userDialogId);

        Task<bool> PauseLastUserDialog(long userId);

        Task<List<DialogStep>> GetDialogSteps(long userDialogId);
        Task<UserDialog> GetUserDialogById(long userDialogId);
        Task<DialogType> GetDialogType(long userDialogId);

        string GetDialogTypeFullForm(UserDialog userDialog);
    }
}
