using System;
using LeeraJenkins.Model.Enum;
using LeeraJenkins.Model.Notifications;
using LeeraJenkins.Common.Extentions;
using LeeraJenkins.Common.Helpers;
using LeeraJenkins.Resources;
using LeeraJenkins.Logic.Helpers;
using LeeraJenkins.Model.Core;

namespace LeeraJenkins.Logic.Notification
{
    public class NotificationMessageGenerator : INotificationMessageGenerator
    {
        public string GenerateRecepientNotificationMessage(NotificationBaseModel model, GameRegistration game,
            NotificationType notificationType, string recepientFullName)
        {
            var recepientAsPlayer = ParseHelper.FromString(recepientFullName);
            var recepientAndOldValueRelationship = recepientAsPlayer.GetRelationshipWith(ParseHelper.FromString(model.OldValue));
            var recepientAndNewValueRelationship = recepientAsPlayer.GetRelationshipWith(ParseHelper.FromString(model.Value));
            var recepientAndHostRelationship = recepientAsPlayer.GetRelationshipWith(game.Host);

            bool recepientIsOldValue = recepientAndOldValueRelationship == PlayerRelationship.Same;
            bool recepientIsNewValue = recepientAndNewValueRelationship == PlayerRelationship.Same;
            bool recepientIsHost = recepientAndHostRelationship == PlayerRelationship.Same;

            var newSubjectForm1 = GetPlayerFullCaseForm(recepientAndNewValueRelationship, 1, $"{model.Value}");
            var oldSubjectForm1 = GetPlayerFullCaseForm(recepientAndOldValueRelationship, 1, $"{model.OldValue}");
            var oldSubjectForm2 = GetPlayerFullCaseForm(recepientAndOldValueRelationship, 2, $"{model.OldValue}");

            var gameInfoPartiallyMessage = $"игру <b>{game.Name}</b> <b>({game.GetDateMessageString()})</b>";

            var message = String.Empty;
            switch (notificationType)
            {
                case NotificationType.PlayerRegistered:
                    message = $"✅ <b>{newSubjectForm1.ToFirstUpperLetter()}</b> записался на {gameInfoPartiallyMessage}";
                    break;
                case NotificationType.PlayerChanged:
                    message = $"⚠️ <b>{newSubjectForm1.ToFirstUpperLetter()}</b> записался вместо <b>{oldSubjectForm2}</b> на {gameInfoPartiallyMessage}";
                    break;
                case NotificationType.PlayerDeleted:
                    message = $"➖ <b>{oldSubjectForm1.ToFirstUpperLetter()}</b> освободил место на {gameInfoPartiallyMessage}";
                    break;
                case NotificationType.HostChanged:
                    message = $"⚠️ У твоей игры <b>{game.Name}</b> сменился хост. Теперь это <b>{newSubjectForm1}</b>";
                    break;
                case NotificationType.TimeChanged:
                    message = $"🕖 Изменилось время игры <b>{game.Name}</b> на <b>{model.Value}</b>";
                    break;
                case NotificationType.GameCreated:
                    message = recepientIsHost
                        ? $"✅ Ты записан хостом на новую {gameInfoPartiallyMessage}"
                        : $"✅ Ты записан игроком на новую {gameInfoPartiallyMessage}";
                    break;
                case NotificationType.GameDeleted:
                    message = recepientIsHost
                        ? $"❌ Ты полностью отменил {gameInfoPartiallyMessage}"
                        : $"❌ Хост <b>{game.Host.TgNickname}</b> полностью отменил {gameInfoPartiallyMessage}. Свяжись с хостом, чтобы узнать подробности";
                    break;
                default:
                    message = "";
                    break;
            }

            if (model.IsExceedPlayer &&
                (notificationType == NotificationType.PlayerRegistered
                || notificationType == NotificationType.PlayerChanged)
                )
            {
                var exceedPlayerPartiallyMessage = recepientIsNewValue
                    ? $"⚠️ Внимание! Ты записался с превышением максимального количества мест для записи!"
                    : $"⚠️ Внимание! Этот игрок записался с превышением максимального количества мест для записи!";

                message = $"{message}{Environment.NewLine}{Environment.NewLine}" + exceedPlayerPartiallyMessage;

                if (recepientIsHost && !recepientIsNewValue)
                {
                    message = $"{message}{Environment.NewLine}" +
                        $"Постарайся связаться с игроком и объяснить, что места на игру закончились";
                }
            }

            return message;
        }

        private static string GetPlayerFullCaseForm(PlayerRelationship relationship, int caseWordForm, string otherPlayerValue)
        {
            var playerWordCaseForm = GetPlayerWordCaseForm(relationship, caseWordForm);
            return playerWordCaseForm +
                (relationship == PlayerRelationship.Same || relationship == PlayerRelationship.OtherIsEmpty
                    ? ""
                    : $" { otherPlayerValue }");
        }

        private static string GetPlayerWordCaseForm(PlayerRelationship relationship, int caseWordForm)
        {
            var playerTypeName = string.Empty;
            switch (relationship)
            {
                case PlayerRelationship.Same:
                    playerTypeName = CaseWordForms.You;
                    break;
                case PlayerRelationship.Friend:
                    playerTypeName = CaseWordForms.YourFriend;
                    break;
                case PlayerRelationship.Other:
                    playerTypeName = CaseWordForms.Player;
                    break;
                case PlayerRelationship.OtherIsEmpty:
                    playerTypeName = CaseWordForms.OtherIsEmpty;
                    break;
            }

            return WordFormHelper.GetFullCasePhrase(caseWordForm, playerTypeName);
        }
    }
}
