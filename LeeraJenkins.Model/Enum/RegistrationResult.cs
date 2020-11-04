using System.ComponentModel;

namespace LeeraJenkins.Model.Enum
{
    public enum RegistrationResult
    {
        [Description("Ошибка выполнения")]
        Success = 0,

        [Description("Ошибка выполнения")]
        Error = 1,

        [Description("Привет, @{0}! Посмотри основные команды, введя \"/\", или используй кнопки снизу")]
        SuccessfulRegistration = 2,

        [Description("Привет, @{0}, я тебя помню! Посмотри основные команды, введя \"/\", или используй кнопки снизу")]
        UserExists = 3,

        [Description("Я не могу зарегистрировать тебя, пока ты не установишь себе телеграм-никнейм")]
        UserHasNoTelegramName = 4
    }
}
