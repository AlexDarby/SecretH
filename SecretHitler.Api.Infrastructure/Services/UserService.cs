using System.IO;
using SecretHitler.Api.Infrastructure.Models;
using Newtonsoft.Json;

namespace SecretHitler.Api.Infrastructure.Services
{
    public class UserService
    {
        public UserService()
        {
        }

        public User Get(string name)
        {
            Users users = JsonConvert.DeserializeObject<Users>(File.ReadAllText("Users.json"));
            return new User
            {
                Name = name,
                Score = users.UserDict[name]
            };
        }
    }
}
