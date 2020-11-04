using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using LeeraJenkins.Logic.Game;
using LeeraJenkins.Logic.Logger;
using LeeraJenkins.Model.Core;

namespace LeeraJenkins.Controllers
{
    [RoutePrefix("api/update")]
    public class UpdateDataController : ApiController
    {
        private IGameLogic _gameLogic;
        private ILogger _logger;

        public UpdateDataController(IGameLogic gameLogic, ILogger logger)
        {
            _gameLogic = gameLogic;
            _logger = logger;
        }

        [HttpGet]
        [Route("get")]
        public string Get()
        {
            return "Get";
        }

        [HttpPost]
        [Route("games")]
        public async Task UpdateGames()
        {
            try
            {
                var games = _gameLogic.GetGameList();
                await Task.Factory.StartNew(() => WaitThenSaveGames(games));
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при обновлении списка игр в БД", ex);
            }
        }

        [HttpPost]
        [Route("games/statuses")]
        public async Task RegularUpdateGamesStatuses()
        {
            try
            {
                await _gameLogic.UpdateFinishedGamesStatuses();
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при регулярном обновлении статусов игр в БД", ex);
            }
        }

        private async Task WaitThenSaveGames(IEnumerable<GameRegistration> games)
        {
            await Task.Delay(1 * 1000);
            await _gameLogic.SaveAllGamesRegistrations(games, true);
        }
    }
}
