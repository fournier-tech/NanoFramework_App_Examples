using System;

namespace NF_test_app_2022
{
    public class Program
    {
        public static void Main()
        {
            GPIOcontrol gpio = new();
            gpio.BlinkLed();

            //******** Used with System.Diagnostics *******
            //Debug.WriteLine("Hello from nanoFramework!");

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }
    }
}