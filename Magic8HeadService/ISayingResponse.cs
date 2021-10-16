using System.Threading.Tasks;

namespace Magic8HeadService
{
    public interface ISayingResponse
    {
        string PickSaying();
        string PickSaying(string mood);
        Task SaySomethingNice(string message);
        Task SetupSayingsAsync();
    }
}
