using Microsoft.Extensions.Configuration.UserSecrets;
using MrBigHead.Shared;
using System;
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

            await Console.Out.WriteLineAsync("Principle is not null");
            AccessToken = principal?.Claims?.FirstOrDefault(c => c.Type == "idp_access_token")?.Value;
            await Console.Out.WriteLineAsync($"AccessToken: {AccessToken}|");
            await Console.Out.WriteLineAsync($"principal: {principal}, Claims: {principal?.Claims}");
            await Console.Out.WriteLineAsync("claims:");
            foreach (var claim in principal.Claims)
            {
                await Console.Out.WriteLineAsync($"claim: {claim.Type} Value: {claim.Value}");
            }

            if (string.IsNullOrEmpty(UserInformation?.UserName))
            {
                UserInformation = await http.GetFromJsonAsync<UserInformation>($"https://bigheadfuncs.azurewebsites.net/api/GetTwitchUserInfo?accessToken={AccessToken}");
            }
            else
            {
                await Console.Out.WriteLineAsync($"return whatever userinformation we have.");
                return UserInformation;
            }

            await Console.Out.WriteLineAsync($"Image: {UserInformation.ImageUrl}");
            return UserInformation;
        }
    }
}
