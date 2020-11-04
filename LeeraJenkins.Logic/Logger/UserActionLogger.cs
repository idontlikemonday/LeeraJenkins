using AutoMapper;
using System;
using LeeraJenkins.DbRepository.Logger;
using LeeraJenkins.Model.Logger;

namespace LeeraJenkins.Logic.Logger
{
    public class UserActionLogger : IUserActionLogger
    {
        private IUserActionLoggerRepository _repository;
        private IMapper _mapper;

        public UserActionLogger(IUserActionLoggerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public void AddUserAction(string username, string action)
        {
            var userAction = new UserActionLog()
            {
                Username = username,
                Action = action,
                Date = DateTime.Now,
            };

            var dbUserAction = _mapper.Map<Db.UserAction>(userAction);

            _repository.AddUserAction(dbUserAction);
        }
    }
}
