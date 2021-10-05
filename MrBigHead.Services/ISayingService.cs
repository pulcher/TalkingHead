using MrBigHead.Shared;
using System.Collections.Generic;

namespace MrBigHead.Services
{
    public interface ISayingService
    {
        List<Saying> GetAllSayings();
    }
}