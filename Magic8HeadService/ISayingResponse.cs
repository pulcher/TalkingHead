using MrBigHead.Shared;
using System.Threading.Tasks;

namespace Magic8HeadService
{
    public interface ISayingResponse
    {
        string PickSaying();
        string PickSaying(string mood);
        Task SaySomethingNice(string message, CommandTrackerEntry commandTrackerEntity = null);
        Task SetupSayingsAsync();
        Task SetupVoiceListAsync();
    }
}
