using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MrBigHead.Web.Services
{
    public class UserInformationProvider(HttpClient http)
    {
        private readonly HttpClient http = http;
        private string AccessToken;


        public async Task<UserInformation> GetUserInformation(ClaimsPrincipal principal)
        {
            if (principal == null) return new UserInformation();

            AccessToken = principal.Claims.FirstOrDefault(c => c.Type == "idp_access_token").Value;

            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            http.DefaultRequestHeaders.Add("Client-Id", "0g0fevdsilc7mbuzgplbgrhg8kowzc");

            var response = await http.GetAsync("https://api.twitch.tv/helix/users");

            var userInfomation = new UserInformation
            {
                UserName = "blah-dude",
                Junk = AccessToken
            };

            return userInfomation;
        }
    }
}
