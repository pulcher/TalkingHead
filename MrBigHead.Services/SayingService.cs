using Microsoft.Extensions.Logging;
using MrBigHead.Shared;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MrBigHead.Services
{
    public class SayingService : ISayingService
    {
        private List<Saying> sayings;
        private ILogger logger;
        private HttpClient client;
        private readonly IHttpClientFactory httpClientFactory;

        public SayingService(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
        {
            this.sayings = new List<Saying>();
            this.logger = loggerFactory.CreateLogger("Generic Logger");

            this.client = httpClientFactory.CreateClient();

            // do this until the get work from the functions
            //this.sayings = GetDefaultSaying();
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<IList<Saying>> GetAllSayingsAsync()
        {
            var sayings = await client.GetFromJsonAsync<IList<Saying>>("https://bigheadfuncs.azurewebsites.net/api/GetAllPhrases");

            return sayings;
        }

        //private static IList<Saying> GetDefaultSaying()
        //{

        //    return new List<Saying>();
        //    //return new List<Saying>
        //    //{
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "Greetings Programs!" },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "Have a nice Day" },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "Ralph helps sooooooo much!" },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "I think you shouldn't have gotten out of bed today!" },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "Yeah, Baby!" },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "I am positive it is gonna suck!" },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "Yes" },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "No" },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "It depends" },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "You want frys with that?" },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "You need to to rephrase your question." },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "If had a dollar for every smart thing you say. I'd be poor." },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "Silence is golden. Duct tape is silver." },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "Id agree with you but then wed both be wrong." },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "Without a doubt!" },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "Make it so!" },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "That is the best idea I have ever heard." },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "Your trending upward!" },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "I would find a scapegoat!" },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "I would wait till tomorrow on that." },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "You need to turn that up to 11!" },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "How would your mother answer that?" },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "You can never touch the piece of water twice." },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "Up is down, down is up, and sideways is straight ahead!" },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "I will answer you tomorrow." },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "Catastrophies are always emminent!" },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "Dogsasters happen at the worst of times!" },
        //    //    new Saying { Mood = Moods.Snarky, Phrase = "Everything happens for a reason, sometimes the reason is you're stupid and make bad decisions." }
        //    //};
        //}
    }
}
