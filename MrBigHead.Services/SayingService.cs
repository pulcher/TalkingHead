using MrBigHead.Shared;
using System;
using System.Collections.Generic;

namespace MrBigHead.Services
{
    public class SayingService : ISayingService
    {
        private List<Saying> sayings;

        public SayingService()
        {
            this.sayings = new List<Saying>();

            // do this until the get work from the functions
            this.sayings = GetDefaultSaying();
        }

        public List<Saying> GetAllSayings() => GetDefaultSaying();

        private static List<Saying> GetDefaultSaying()
        {
            return new List<Saying>
            {
                new Saying { Mood = Moods.Snarky, Quip = "Greetings Programs!" },
                new Saying { Mood = Moods.Snarky, Quip = "Have a nice Day" },
                new Saying { Mood = Moods.Snarky, Quip = "Ralph helps sooooooo much!" },
                new Saying { Mood = Moods.Snarky, Quip = "I think you shouldn't have gotten out of bed today!" },
                new Saying { Mood = Moods.Snarky, Quip = "Yeah, Baby!" },
                new Saying { Mood = Moods.Snarky, Quip = "I am positive it is gonna suck!" },
                new Saying { Mood = Moods.Snarky, Quip = "Yes" },
                new Saying { Mood = Moods.Snarky, Quip = "No" },
                new Saying { Mood = Moods.Snarky, Quip = "It depends" },
                new Saying { Mood = Moods.Snarky, Quip = "You want frys with that?" },
                new Saying { Mood = Moods.Snarky, Quip = "You need to to rephrase your question." },
                new Saying { Mood = Moods.Snarky, Quip = "If had a dollar for every smart thing you say. I'd be poor." },
                new Saying { Mood = Moods.Snarky, Quip = "Silence is golden. Duct tape is silver." },
                new Saying { Mood = Moods.Snarky, Quip = "Id agree with you but then wed both be wrong." },
                new Saying { Mood = Moods.Snarky, Quip = "Without a doubt!" },
                new Saying { Mood = Moods.Snarky, Quip = "Make it so!" },
                new Saying { Mood = Moods.Snarky, Quip = "That is the best idea I have ever heard." },
                new Saying { Mood = Moods.Snarky, Quip = "Your trending upward!" },
                new Saying { Mood = Moods.Snarky, Quip = "I would find a scapegoat!" },
                new Saying { Mood = Moods.Snarky, Quip = "I would wait till tomorrow on that." },
                new Saying { Mood = Moods.Snarky, Quip = "You need to turn that up to 11!" },
                new Saying { Mood = Moods.Snarky, Quip = "How would your mother answer that?" },
                new Saying { Mood = Moods.Snarky, Quip = "You can never touch the piece of water twice." },
                new Saying { Mood = Moods.Snarky, Quip = "Up is down, down is up, and sideways is straight ahead!" },
                new Saying { Mood = Moods.Snarky, Quip = "I will answer you tomorrow." },
                new Saying { Mood = Moods.Snarky, Quip = "Catastrophies are always emminent!" },
                new Saying { Mood = Moods.Snarky, Quip = "Dogsasters happen at the worst of times!" },
                new Saying { Mood = Moods.Snarky, Quip = "Everything happens for a reason, sometimes the reason is you're stupid and make bad decisions." }
            };
        }
    }
}
