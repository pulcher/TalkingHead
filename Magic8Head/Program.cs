using System;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace Magic8Head
{
    class Program
    {
        static int buttonPin = 7;
        static GpioController controller;
        static List<string> sayings;
        static Random random;

        static async Task Main(string[] args)
        {
            SetupGPIO();

            SetupSayings();

            while (true)
            {
                var status = controller.Read(buttonPin);
                
                if (status == PinValue.Low)
                    SaySomethingNice();
                    
                Thread.Sleep (200);
            }
        }

        public static void SetupGPIO()
        {
            System.Console.WriteLine("Setiing up GPIO...");
            controller = new GpioController(0, new RaspberryPi3Driver());

            controller.OpenPin (buttonPin, PinMode.InputPullUp);
        }

        public static void SetupSayings()
        {
            System.Console.WriteLine("Setiing up Sayings...");

            random = new Random();
            sayings = new List<string>
            {
                "Greetings Programs!",
                "Have a nice Day",
                "Ralph helps sooooooo much!"
            };
        }

        public static void SaySomethingNice()
        {
            using (Process fliteProcess = new Process())
            {
                var voiceDir = "/home/pi/work/speechTest/voices/";
                var voice = "cmu_us_aew.flitevox";
                var exec = "/usr/local/bin/flite";
                var args = $"-voice awb";

                fliteProcess.StartInfo.FileName = exec;
                fliteProcess.StartInfo.Arguments = args;
                fliteProcess.StartInfo.UseShellExecute = false;
                fliteProcess.StartInfo.RedirectStandardInput = true;

                fliteProcess.Start();

                var streamWriter = fliteProcess.StandardInput;
                
                var inputText = PickSaying();
                System.Console.WriteLine($"Saying: {inputText}");

                streamWriter.WriteLine(inputText);
                streamWriter.Close();

                fliteProcess.WaitForExit();
            }

            return;
        }

        public static string PickSaying()
        {
            var index = random.Next(sayings.Count);
            System.Console.WriteLine($"count: {sayings.Count}, random: {index}");
            var picked = sayings[index];

            return picked;
        }
    }
}
