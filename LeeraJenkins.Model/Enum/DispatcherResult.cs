using System.ComponentModel;

namespace LeeraJenkins.Model.Enum
{
    public enum DispatcherResult
    {
        NoResult = -1,

        Success = 0,

        [Description("Ошибка выполнения")]
        Error = 1,

        //[Description("Ура! Ты успешно записан на игру")]
        SuccessfulGameRegistration = 2,

        [Description("Игра для записи не найдена. Попробуй снова получить список свободных игр")]
        RegistrtingGameNotFound = 3,

        [Description("В игре не осталось свободных мест. Похоже, ты не успел")]
        VacantPlaceNotFound = 4,

        [Description("Ошибка записи на игру. Попробуй еще раз или проверь список свободных игр")]
        GameRegistrationError = 5,

        [Description("Все в порядке, ты уже записан на эту игру")]
        PlayerAlreadyRegistered = 6,

        //[Description("Ты успешно отменил запись на игру! Постарайся не злоупотреблять в будущем :)")]
        SuccessfulGameDeRegistration = 7,

        [Description("Игра для отмены записи не найдена. Попробуй снова получить список игр, на которые ты записан")]
        DeRegistrtingGameNotFound = 8,

        [Description("Я не вижу, чтобы ты был записан на эту игру. Попробуй снова получить список игр, на которые ты записан")]
        RegisteredPlaceNotFound = 9,

        [Description("У меня нет полномочий выписывать хоста с его же игры. Если ты хочешь отменить игру полностью, воспользуйся кнопкой полной отмены игры")]
        CantDeRegisterHost = 10,

        [Description("Я освободила твое место от участия в игре, но учти, что ты все еще остаешься хостом игры!")]
        HostAndPlayerDeRegisterWarning = 11,

        [Description("Ошибка отмены записи на игру. Попробуй еще раз или проверь список игр, на которые ты записан")]
        GameDeRegistrationError = 12,

        FullGameDeletion = 13,

        [Description("⚠️ Значение не соответствует формату. Введи правильное значение")]
        ValidationError = 101,

        //[Description("Не могу выполнить. Тебе нужно зарегистрироваться с помощью команды /start")]
        NullUser = 102,

        NoActiveDialogs = 103,

        [Description("⚠️ Это прошедшая дата. Выбери дату, начиная с сегодняшнего дня")]
        DateFromPast = 104,

        [Description("✅ Новая игра занесена в табличку")]
        Finalized = 105,
    }
}
