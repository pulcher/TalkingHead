using Microsoft.Extensions.Configuration;

namespace Magic8HeadService.Options;

public record VoiceConfiguration
{
  [ConfigurationKeyName("Language")]
  public string Language { get; set; }

  [ConfigurationKeyName("VoiceName")]
  public string VoiceName { get; set; }
}