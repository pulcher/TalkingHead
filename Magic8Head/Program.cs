using System;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Threading;
using System.Threading.Tasks;

namespace Magic8Head
{
    class Program
    {
        static int buttonPin = 7;
        static GpioController controller;

        static async Task Main(string[] args)
        {
            SetupGPIO();

            while (true)
            {
                var status = controller.Read(buttonPin);
                
                if (status == PinValue.Low)
                    SaySomethingNice();
                    
                Thread.Sleep (100);
            }
        }

        public static void SetupGPIO()
        {
            System.Console.WriteLine("Setiing up GPIO...");
            controller = new GpioController(0, new RaspberryPi3Driver());

            controller.OpenPin (buttonPin, PinMode.InputPullUp);
        }

        public static void SaySomethingNice()
        {
            System.Console.WriteLine("Say something!!!");
            return;
        }
    }
}
