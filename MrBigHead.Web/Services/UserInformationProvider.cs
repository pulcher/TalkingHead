using MrBigHead.Shared;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MrBigHead.Web.Services
{
    public class UserInformationProvider(HttpClient http)
    {
        private readonly HttpClient http = http;
        private string AccessToken;
        private UserInformation UserInformation = new();

        public async Task<UserInformation> GetUserInformation(ClaimsPrincipal principal)
        {
            if (principal == null) return new UserInformation();

            AccessToken = principal.Claims.FirstOrDefault(c => c.Type == "idp_access_token").Value;

            if (string.IsNullOrEmpty(UserInformation.UserName))
            {
                UserInformation = await http.GetFromJsonAsync<UserInformation>($"https://bigheadfuncs.azurewebsites.net/api/GetTwitchUserInfo?accessToken={AccessToken}");
            }

            return UserInformation;
        }
    }
}
