using System.Threading.Tasks;

namespace Magic8HeadService
{
    public interface ISayingResponse
    {
        string Attitude { get; set; }

        string PickSaying();
        Task SaySomethingNice(string message);
        void SetupSayings();
    }
}