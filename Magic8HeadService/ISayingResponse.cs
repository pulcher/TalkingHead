using MrBigHead.Shared;
using System.Threading.Tasks;
using TwitchLib.Client.Interfaces;

namespace Magic8HeadService
{
    public interface ISayingResponse
    {
        string PickSaying();
        string PickSaying(string mood);
        Task SaySomethingNiceAsync(string message, ITwitchClient client, string channel, 
            string username, CommandTrackerEntry commandTrackerEntity = null);
        Task SetupSayingsAsync();
        Task SetupVoiceListAsync();
    }
}
