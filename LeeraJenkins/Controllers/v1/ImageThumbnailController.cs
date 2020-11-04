using System.Threading.Tasks;
using System.Web.Http;
using LeeraJenkins.Logic.Cache;
using LeeraJenkins.Logic.Logger;
using LeeraJenkins.Logic.User;
using LeeraJenkins.Model.ApiModel.Base;
using OpenGraphNet;

namespace LeeraJenkins.Controllers
{
    [RoutePrefix("api/v1")]
    public class ImageThumbnailController : ApiController
    {
        private ICacheLogic _cache;
        private IUserLogic _userLogic;
        private ILogger _logger;

        public ImageThumbnailController(ICacheLogic cache, IUserLogic userLogic, ILogger logger)
        {
            _cache = cache;
            _userLogic = userLogic;
            _logger = logger;
        }

        [HttpGet]
        [Route("imageUrl")]
        public async Task<ApiResponse<string>> GetImageThumbnail([FromUri] string url)
        {
            if (_cache.IsExists(url))
            {
                return ApiResponse.Ok(_cache.Get(url));
            }

            var graph = await OpenGraph.ParseUrlAsync(url);
            var result = graph.Image.AbsoluteUri;
            _cache.Set(url, result);
            return ApiResponse.Ok(result);
        }

        [HttpGet]
        [Route("userPhoto")]
        public async Task<ApiResponse<string>> GetUserPhoto()
        {
            await _userLogic.GetPhoto();

            return ApiResponse.Ok("");
        }
    }
}
