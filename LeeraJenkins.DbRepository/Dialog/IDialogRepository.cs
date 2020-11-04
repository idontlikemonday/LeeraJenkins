using System.Collections.Generic;
using System.Threading.Tasks;
using LeeraJenkins.Db;
using LeeraJenkins.Model.Enum;

namespace LeeraJenkins.DbRepository.Dialog
{
    public interface IDialogRepository
    {
        Task<UserDialog> InitiateDialog(UserDialog userDialog);
        Task AddStep(DialogStep step);
        Task<UserDialog> GetLastDialogWithStatus(long userId, DialogStatus status);
        Task<UserDialog> GetLastActiveOrPausedDialog(long userId);
        Task<UserDialog> GetUserDialog(long userDialogId);
        Task<DialogStep> GetLastUserDialogStep(long userDialogId);
        Task<List<DialogStep>> GetDialogSteps(long userDialogId);
        Task SetStatusForUserDialog(long userDialogId, DialogStatus status);
    }
}
