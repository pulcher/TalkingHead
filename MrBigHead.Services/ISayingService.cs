using MrBigHead.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrBigHead.Services
{
    public interface ISayingService
    {
        Task<List<Saying>> GetAllSayingsAsync();
    }
}