using System.ComponentModel;

namespace LeeraJenkins.Model.Enum
{
    public enum DialogDispatcherResult
    {
        NoResult = -1,

        Success = 0,

        [Description("⚠️ Значение не соответствует формату. Введи правильное значение")]
        ValidationError = 1,

        //[Description("Не могу выполнить. Тебе нужно зарегистрироваться с помощью команды /start")]
        NullUser = 2,

        NoActiveDialogs = 3,

        [Description("⚠️ Это прошедшая дата. Выбери дату, начиная с сегодняшнего дня")]
        DateFromPast = 4,

        [Description("✅ Новая игра занесена в табличку")]
        Finalized = 5
    }
}
