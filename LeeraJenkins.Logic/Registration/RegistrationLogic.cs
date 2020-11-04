using AutoMapper;
using System;
using System.Threading.Tasks;
using LeeraJenkins.DbRepository.User;
using LeeraJenkins.Model.Enum;

namespace LeeraJenkins.Logic.Registration
{
    using User = LeeraJenkins.Model.Core.User;

    public class RegistrationLogic : IRegistrationLogic
    {
        private IUserRepository _userRepository;
        private IMapper _mapper;

        public RegistrationLogic(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<RegistrationResult> RegisterUser(User user)
        {
            if (user.TelegramName != null && String.IsNullOrEmpty(user.TelegramName.Trim(new char[] { '@' })))
            {
                return RegistrationResult.UserHasNoTelegramName;
            }

            var dbUser = _mapper.Map<Db.User>(user);
            dbUser.Date = DateTime.Now;

            var isExists = await _userRepository.CheckRegistration(dbUser);
            if (isExists)
            {
                return RegistrationResult.UserExists;
            }
            await _userRepository.RegisterUser(dbUser);
            return RegistrationResult.SuccessfulRegistration;
        }
    }
}
