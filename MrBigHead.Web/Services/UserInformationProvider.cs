using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using static MrBigHead.Web.Pages.Quips;
using static System.Net.WebRequestMethods;

namespace MrBigHead.Web.Services
{
    public class UserInformationProvider(HttpClient http)
    {
        private readonly HttpClient http = http;
        private string AccessToken;
        private string ClientId;


        public async Task<UserInformation> GetUserInformation(ClaimsPrincipal principal, string clientId)
        {
            if (principal == null) return new UserInformation();

            AccessToken = principal.Claims.FirstOrDefault(c => c.Type == "idp_access_token").Value;

            //var twitchClientId = configuration["TwitchAPI_ClientId"];

            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            http.DefaultRequestHeaders.Add("Client-Id", clientId);

            //var userStuff = await Http.GetFromJsonAsync<TwitchUser>("https://bigheadfuncs.azurewebsites.net/api/getallphrases");

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
