using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using LeeraJenkins.Common.Extentions;
using LeeraJenkins.Logic.Game;
using LeeraJenkins.Logic.Helpers;
using LeeraJenkins.Logic.Logger;
using LeeraJenkins.Model.ApiModel;
using LeeraJenkins.Model.ApiModel.Base;
using LeeraJenkins.Model.Core;
using LeeraJenkins.Model.Result;

namespace LeeraJenkins.Controllers
{
    [RoutePrefix("api/v1/games")]
    public class GameController : ApiController
    {
        private IGameLogic _gameLogic;
        private ILogger _logger;

        public GameController(IGameLogic gameLogic, ILogger logger)
        {
            _gameLogic = gameLogic;
            _logger = logger;
        }

        /// <summary>
        /// Get all games from g-sheet table
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("all")]
        public ApiResponse<IEnumerable<GameRegistration>> GetGameList()
        {
            var result = _gameLogic.GetGameList();
            return ApiResponse.Ok(result);
        }

        /// <summary>
        /// Get all games from g-sheet table for player with playerName
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("all/{playerName}")]
        public ApiResponse<IEnumerable<GameRegistration>> GetGameList(string playerName = null)
        {
            var result = _gameLogic.GetGameList(playerName);
            return ApiResponse.Ok(result);
        }

        /// <summary>
        /// Get games from g-sheet table having at least 1 empty slot
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("vacant")]
        public ApiResponse<IEnumerable<GameRegistration>> GetVacantGameList()
        {
            var result = _gameLogic.GetVacantGameList();
            return ApiResponse.Ok(result);
        }

        /// <summary>
        /// Get games from g-sheet table having at least 1 empty slot for player with playerName
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("vacant/{playerName}")]
        public ApiResponse<IEnumerable<GameRegistration>> GetVacantGameList(string playerName = null)
        {
            var result = _gameLogic.GetVacantGameList(playerName);
            return ApiResponse.Ok(result);
        }

        [HttpPost]
        [Route("register")]
        public async Task<ApiResponse<DispatcherDescriptionResult>> RegisterPlayerToGame([FromBody] RegistrationModel model)
        {
            var player = ParseHelper.FromString(model.PlayerName);
            var resultCode = await _gameLogic.RegisterPlayerToGame(player, model.SheetRowId, model.TrimmedGameName, model.PlayerSlotNum);
            var result = new DispatcherDescriptionResult(resultCode);

            return ApiResponse.Ok(result);
        }

        [HttpPost]
        [Route("deregister")]
        public async Task<ApiResponse<DispatcherDescriptionResult>> DeregisterPlayerFromGame([FromBody] DeregistrationModel model)
        {
            var player = ParseHelper.FromString(model.PlayerName);
            var resultCode = await _gameLogic.DeRegisterPlayerToGame(player, model.SheetRowId, model.WithFullGameDeletion, model.TrimmedGameName);
            var result = new DispatcherDescriptionResult(resultCode);

            return ApiResponse.Ok(result);
        }
    }
}
