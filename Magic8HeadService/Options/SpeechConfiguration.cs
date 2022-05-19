using Microsoft.Extensions.Configuration;

namespace Magic8HeadService.Options;

public class SpeechConfiguration
{
  [ConfigurationKeyName("Subscription")]
  public string Subscription { get; set; }
  [ConfigurationKeyName("Region")]
  public string ServiceRegion { get; set; }
}