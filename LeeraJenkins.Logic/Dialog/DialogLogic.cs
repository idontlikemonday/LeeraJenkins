using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeeraJenkins.DbRepository.Dialog;
using LeeraJenkins.Logic.Helpers;
using LeeraJenkins.Model.Dialog;
using LeeraJenkins.Model.Enum;
using LeeraJenkins.Resources;

namespace LeeraJenkins.Logic.Dialog
{
    public class DialogLogic : IDialogLogic
    {
        private IMapper _mapper;
        private IDialogRepository _dialogRepository;

        public DialogLogic(IMapper mapper, IDialogRepository dialogRepository)
        {
            _mapper = mapper;
            _dialogRepository = dialogRepository;
        }

        public async Task<UserDialog> InitiateDialog(UserDialog userDialog)
        {
            var dbUserDialog = _mapper.Map<Db.UserDialog>(userDialog);
            var result = await _dialogRepository.InitiateDialog(dbUserDialog);
            return _mapper.Map<UserDialog>(result);
        }

        public async Task AddStep(DialogStep step)
        {
            await _dialogRepository.AddStep(_mapper.Map<Db.DialogStep>(step));
        }

        public async Task<UserDialog> GetLastActiveDialog(long userId)
        {
            var dbDialog = await _dialogRepository.GetLastDialogWithStatus(userId, DialogStatus.Active);
            if (dbDialog == null)
            {
                return null;
            }
            return _mapper.Map<UserDialog>(dbDialog);
        }

        public async Task<UserDialog> GetLastPausedDialog(long userId)
        {
            var dbDialog = await _dialogRepository.GetLastDialogWithStatus(userId, DialogStatus.Paused);
            if (dbDialog == null)
            {
                return null;
            }
            return _mapper.Map<UserDialog>(dbDialog);
        }

        public async Task<UserDialog> GetLastActiveOrPausedDialog(long userId)
        {
            var dbDialog = await _dialogRepository.GetLastActiveOrPausedDialog(userId);
            if (dbDialog == null)
            {
                return null;
            }
            return _mapper.Map<UserDialog>(dbDialog);
        }

        public async Task<DialogStep> GetLastDialogStep(long userDialogId)
        {
            var dbStep = await _dialogRepository.GetLastUserDialogStep(userDialogId);
            if (dbStep == null)
            {
                return null;
            }
            return _mapper.Map<DialogStep>(dbStep);
        }

        public async Task<DialogStep> GetLastDialogStepForUser(long userId)
        {
            var lastDialog = await GetLastActiveDialog(userId);
            if (lastDialog == null)
            {
                return null;
            }

            return await GetLastDialogStep(lastDialog.Id);
        }

        public async Task ContinueUserDialog(long userDialogId)
        {
            await _dialogRepository.SetStatusForUserDialog(userDialogId, Model.Enum.DialogStatus.Active);
        }

        public async Task PauseUserDialog(long userDialogId)
        {
            await _dialogRepository.SetStatusForUserDialog(userDialogId, Model.Enum.DialogStatus.Paused);
        }

        public async Task<bool> PauseLastUserDialog(long userId)
        {
            var lastDialog = await GetLastActiveDialog(userId);
            if (lastDialog != null)
            {
                await PauseUserDialog(lastDialog.Id);
                return true;
            }
            return false;
        }

        public async Task FinalizeUserDialog(long userDialogId)
        {
            await _dialogRepository.SetStatusForUserDialog(userDialogId, Model.Enum.DialogStatus.Finished);
        }

        public async Task<List<DialogStep>> GetDialogSteps(long userDialogId)
        {
            var dbSteps = await _dialogRepository.GetDialogSteps(userDialogId);
            return dbSteps.Select(s => _mapper.Map<DialogStep>(s)).ToList();
        }

        public async Task<UserDialog> GetUserDialogById(long userDialogId)
        {
            var dbUserDialog = await _dialogRepository.GetUserDialog(userDialogId);
            return _mapper.Map<UserDialog>(dbUserDialog);
        }

        public async Task<DialogType> GetDialogType(long userDialogId)
        {
            var dialog = await GetUserDialogById(userDialogId);
            return dialog.DialogId;
        }

        public string GetDialogTypeFullForm(UserDialog userDialog)
        {
            int caseNum = 3;

            var caseFromsString = String.Empty;
            switch (userDialog.DialogId)
            {
                case DialogType.NewGame:
                    caseFromsString = CaseWordForms.NewGame;
                    break;
                case DialogType.AddFriendToGame:
                    caseFromsString = CaseWordForms.AddFriendToGame;
                    break;
            }

            var dialogTypeFullForm = WordFormHelper.GetFullCasePhrase(caseNum, caseFromsString);
            return dialogTypeFullForm;
        }
    }
}
