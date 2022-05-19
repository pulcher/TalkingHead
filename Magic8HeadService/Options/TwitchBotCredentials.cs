using Microsoft.Extensions.Configuration;
using TwitchLib.Client.Models;

namespace Magic8HeadService.Options;

public record TwitchBotCredentials
{
  [ConfigurationKeyName("UserName")] public string Username { get; set; }

  [ConfigurationKeyName("ClientId")] public string ClientID { get; set; }
  public string AccessToken { get; set; }
 
  public ConnectionCredentials AsConnectionCredentials() => new ConnectionCredentials(this.Username, this.AccessToken);

  public ConnectionCredentials AsConnectionCredentials(string websocketURI) => new ConnectionCredentials(this.Username, this.AccessToken, websocketURI);

}