using MrBigHead.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrBigHead.Services
{
    public interface ISayingService
    {
        Task<IList<Saying>> GetAllSayingsAsync();
    }
}