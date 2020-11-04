using System.Threading.Tasks;
using LeeraJenkins.DbRepository.User;

namespace LeeraJenkins.Logic.User
{
    using AutoMapper;
    using LeeraJenkins.Logic.Bot;
    using LeeraJenkins.Model.Core;

    public class UserLogic: IUserLogic
    {
        private IUserRepository _userRepository;
        private IBotClient _client;
        private IMapper _mapper;

        public UserLogic(IUserRepository userRepository, IBotClient client, IMapper mapper)
        {
            _userRepository = userRepository;
            _client = client;
            _mapper = mapper;
        }

        public async Task<long?> GetUserId(string tgName)
        {
            return await _userRepository.GetUserId(tgName);
        }

        public async Task<long?> GetUserId(long chatId)
        {
            return await _userRepository.GetUserId(chatId);
        }

        public async Task<User> GetUser(string tgName)
        {
            var dbresult = await _userRepository.GetUser(tgName);
            return _mapper.Map<User>(dbresult);
        }

        public async Task<User> GetUser(long userId)
        {
            var dbresult = await _userRepository.GetUser(userId);
            return _mapper.Map<User>(dbresult);
        }

        public async Task GetPhoto()
        {
            var client = _client.GetClient();
            var photos = await client.GetUserProfilePhotosAsync(397558545, 0);

            if (photos.TotalCount > 0)
            {
                var photo = photos.Photos[0][0];
                var res = await client.GetFileAsync(photo.FileId);
            }
        }
    }
}
