using MrBigHead.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrBigHead.Services
{
    public interface IVoiceService
    {
        Task<IList<Voice>> GetAllVoicesAsync();
    }
}
