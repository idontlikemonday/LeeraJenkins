using System;
using System.Threading.Tasks;
using System.Linq;
using LeeraJenkins.Db;
using System.Collections.Generic;

namespace LeeraJenkins.DbRepository.User
{
    using User = LeeraJenkins.Db.User;

    public class UserRepository : IUserRepository
    {
        public async Task RegisterUser(User user)
        {
            using (var context = new TgEntities())
            {
                var users = context.User;
                users.Add(user);

                context.SaveChanges();
            }
        }

        public async Task<bool> CheckRegistration(User user)
        {
            using (var context = new TgEntities())
            {
                var foundUser = context.User
                    .FirstOrDefault(u => u.TelegramName == user.TelegramName);

                return foundUser != null;
            }
        }

        public async Task<List<User>> GetAllUsers()
        {
            using (var context = new TgEntities())
            {
                return context.User.ToList();
            }
        }

        public async Task<long?> GetChatId(string tgName)
        {
            using (var context = new TgEntities())
            {
                var foundUser = context.User
                    .FirstOrDefault(u => u.TelegramName == tgName);

                return foundUser?.ChatId;
            }
        }

        public async Task<long?> GetUserId(string tgName)
        {
            using (var context = new TgEntities())
            {
                var foundUser = context.User
                    .FirstOrDefault(u => u.TelegramName == tgName);

                return foundUser?.Id;
            }
        }

        public async Task<long?> GetUserId(long chatId)
        {
            using (var context = new TgEntities())
            {
                var foundUser = context.User
                    .FirstOrDefault(u => u.ChatId == chatId);

                return foundUser?.Id;
            }
        }

        public async Task<User> GetUser(string tgName)
        {
            using (var context = new TgEntities())
            {
                var foundUser = context.User
                    .FirstOrDefault(u => u.TelegramName == tgName);

                return foundUser;
            }
        }

        public async Task<User> GetUser(long userId)
        {
            using (var context = new TgEntities())
            {
                var foundUser = context.User
                    .FirstOrDefault(u => u.Id == userId);

                return foundUser;
            }
        }
    }
}
