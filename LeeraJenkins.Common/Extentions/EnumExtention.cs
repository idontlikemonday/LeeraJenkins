using System.ComponentModel;
using System.Linq;

namespace LeeraJenkins.Common.Extentions
{
    public static class EnumExtensions
    {
        public static string GetDescription<T>(this T item)
        {
            var mem = typeof(T).GetMember(item.ToString()).First();
            var desc = mem.GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>().FirstOrDefault();
            if (desc == null)
                return string.Empty;

            return desc.Description;
        }
    }
}
