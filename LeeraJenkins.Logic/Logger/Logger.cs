using System;
using System.Collections.Generic;
using AutoMapper;
using LeeraJenkins.Model.Logger;
using LeeraJenkins.DbRepository.Logger;

namespace LeeraJenkins.Logic.Logger
{
    public class Logger : ILogger
    {
        private ILoggerRepository _repo;
        private IMapper _mapper;

        public Logger(ILoggerRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public void LogError(string message, Exception ex)
        {
            var log = new Log()
            {
                Date = DateTime.Now,
                Message = $"{message}: {ex.Message}",
                StackTrace = ex.StackTrace
            };

            var dbLog = _mapper.Map<Db.Log>(log);

            _repo.AddLog(dbLog);
        }
    }
}
