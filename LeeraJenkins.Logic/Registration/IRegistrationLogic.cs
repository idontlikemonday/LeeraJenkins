using System.Threading.Tasks;
using LeeraJenkins.Model.Enum;

namespace LeeraJenkins.Logic.Registration
{
    using User = LeeraJenkins.Model.Core.User;

    public interface IRegistrationLogic
    {
        Task<RegistrationResult> RegisterUser(User user);
    }
}
