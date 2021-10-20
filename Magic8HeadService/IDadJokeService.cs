using System.Threading.Tasks;

namespace Magic8HeadService
{
    public interface IDadJokeService
    {
        Task<DadJoke> GetDadJoke();
    }
}