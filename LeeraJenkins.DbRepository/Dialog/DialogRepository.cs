using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeeraJenkins.Db;
using LeeraJenkins.Model.Enum;

namespace LeeraJenkins.DbRepository.Dialog
{
    public class DialogRepository : IDialogRepository
    {
        public async Task<UserDialog> InitiateDialog(UserDialog userDialog)
        {
            using (var context = new TgEntities())
            {
                var userDialogs = context.UserDialog;
                var result = userDialogs.Add(userDialog);

                context.SaveChanges();

                return result;
            }
        }

        public async Task AddStep(DialogStep step)
        {
            using (var context = new TgEntities())
            {
                var steps = context.DialogStep;
                var result = steps.Add(step);

                context.SaveChanges();
            }
        }

        public async Task<UserDialog> GetLastDialogWithStatus(long userId, DialogStatus status)
        {
            using (var context = new TgEntities())
            {
                var userDialog = context.UserDialog
                    .OrderByDescending(ud => ud.Date)
                    .FirstOrDefault(ud => ud.UserId == userId && ud.Status == (int)status);

                return userDialog;
            }
        }

        public async Task<UserDialog> GetLastActiveOrPausedDialog(long userId)
        {
            using (var context = new TgEntities())
            {
                var userDialog = context.UserDialog
                    .OrderByDescending(ud => ud.Date)
                    .FirstOrDefault(ud => ud.UserId == userId &&
                        (ud.Status == (int)DialogStatus.Active || ud.Status == (int)DialogStatus.Paused));

                return userDialog;
            }
        }

        public async Task<UserDialog> GetUserDialog(long userDialogId)
        {
            using (var context = new TgEntities())
            {
                var userDialog = context.UserDialog
                    .FirstOrDefault(ud => ud.Id == userDialogId);

                return userDialog;
            }
        }

        public async Task<DialogStep> GetLastUserDialogStep(long userDialogId)
        {
            using (var context = new TgEntities())
            {
                var steps = context.DialogStep
                    .Where(ds => ds.UserDialogId == userDialogId);

                return steps.
                    FirstOrDefault(ds => ds.StepNum == steps.Max(s => s.StepNum));
            }
        }

        public async Task<List<DialogStep>> GetDialogSteps(long userDialogId)
        {
            using (var context = new TgEntities())
            {
                var steps = context.DialogStep
                    .Where(ds => ds.UserDialogId == userDialogId);

                return steps
                    .ToList()
                    .OrderBy(ds => ds.StepNum)
                    .ToList();
            }
        }

        public async Task SetStatusForUserDialog(long userDialogId, DialogStatus status)
        {
            using (var context = new TgEntities())
            {
                var userDialog = context.UserDialog
                    .FirstOrDefault(ud => ud.Id == userDialogId);

                if (userDialog != null)
                {
                    userDialog.Status = (int)status;
                    context.Entry(userDialog).Property(ud => ud.Status).IsModified = true;
                    context.SaveChanges();
                }
            }
        }
    }
}
