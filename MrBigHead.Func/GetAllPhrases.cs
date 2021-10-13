using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using MrBigHead.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrBigHead.Func
{
    public static partial class GetAllPhrases
    {
        [FunctionName("GetAllPhrases")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [Table("Sayings")] CloudTable cloudTable,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var sayings = new List<Saying>(); //GetDefaultPhrases();

            var cloudQuery = new TableQuery<SayingEntity>();

            foreach (var entity in
                await cloudTable.ExecuteQuerySegmentedAsync(cloudQuery, null))
            {
                log.LogInformation($"entity: {entity.Mood}:{entity.Phrase}");
                sayings.Add(new Saying { Mood = entity.Mood, Phrase = entity.Phrase });
            }

            return new JsonResult(GetDefaultPhrases());
        }

        private static List<Saying> GetDefaultPhrases()
        {
            return new List<Saying>
            {
                new Saying { Mood = Moods.Snarky, Phrase = "Greetings Programs!" },
                new Saying { Mood = Moods.Snarky, Phrase = "Have a nice Day" },
                new Saying { Mood = Moods.Snarky, Phrase = "Ralph helps sooooooo much!" },
                new Saying { Mood = Moods.Snarky, Phrase = "I think you shouldn't have gotten out of bed today!" },
                new Saying { Mood = Moods.Snarky, Phrase = "Yeah, Baby!" },
                new Saying { Mood = Moods.Snarky, Phrase = "I am positive it is gonna suck!" },
                new Saying { Mood = Moods.Snarky, Phrase = "Yes" },
                new Saying { Mood = Moods.Snarky, Phrase = "No" },
                new Saying { Mood = Moods.Snarky, Phrase = "It depends" },
                new Saying { Mood = Moods.Snarky, Phrase = "You want frys with that?" },
                new Saying { Mood = Moods.Snarky, Phrase = "You need to to rephrase your question." },
                new Saying { Mood = Moods.Snarky, Phrase = "If had a dollar for every smart thing you say. I'd be poor." },
                new Saying { Mood = Moods.Snarky, Phrase = "Silence is golden. Duct tape is silver." },
                new Saying { Mood = Moods.Snarky, Phrase = "Id agree with you but then wed both be wrong." },
                new Saying { Mood = Moods.Snarky, Phrase = "Without a doubt!" },
                new Saying { Mood = Moods.Snarky, Phrase = "Make it so!" },
                new Saying { Mood = Moods.Snarky, Phrase = "That is the best idea I have ever heard." },
                new Saying { Mood = Moods.Snarky, Phrase = "Your trending upward!" },
                new Saying { Mood = Moods.Snarky, Phrase = "I would find a scapegoat!" },
                new Saying { Mood = Moods.Snarky, Phrase = "I would wait till tomorrow on that." },
                new Saying { Mood = Moods.Snarky, Phrase = "You need to turn that up to 11!" },
                new Saying { Mood = Moods.Snarky, Phrase = "How would your mother answer that?" },
                new Saying { Mood = Moods.Snarky, Phrase = "You can never touch the piece of water twice." },
                new Saying { Mood = Moods.Snarky, Phrase = "Up is down, down is up, and sideways is straight ahead!" },
                new Saying { Mood = Moods.Snarky, Phrase = "I will answer you tomorrow." },
                new Saying { Mood = Moods.Snarky, Phrase = "Catastrophies are always emminent!" },
                new Saying { Mood = Moods.Snarky, Phrase = "Dogsasters happen at the worst of times!" },
                new Saying { Mood = Moods.Snarky, Phrase = "Everything happens for a reason, sometimes the reason is you're stupid and make bad decisions." }
            };
        }

    }
}
