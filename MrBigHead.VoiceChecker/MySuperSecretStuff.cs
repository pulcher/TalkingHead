using System.Diagnostics.CodeAnalysis;

namespace MrBigHead.VoiceChecker
{
    public class MySuperSecretStuff
    {
        [AllowNull]
        public string SpeechSubscription { get; set; }

        [AllowNull]
        public string SpeechServiceRegion { get; set; }

    }
}
