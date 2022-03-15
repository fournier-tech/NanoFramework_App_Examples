using System.Device.Gpio;
using nanoFramework.Hardware.Esp32;

namespace NanoFramework_App_Examples
{
    public class Program
    {
        public static void Main()
        {
            PWM_TV_Remote pWM_TV_Remote = new();
            pWM_TV_Remote.RemoteOperation();
        }
    }
}

//PWM_Test pwm = new();
//pwm.PwmOperation();

//******** call GPIO class instance ***********
//GPIOcontrol gpio = new();
//gpio.BlinkLed();

//******** Used with System.Diagnostics *******
//Debug.WriteLine("Hello from nanoFramework!");

// Check our documentation online: https://docs.nanoframework.net/api/index.html
// Browse our samples repository: https://github.com/nanoframework/samples
// Check our documentation online: https://docs.nanoframework.net/
// Join our lively Discord community: https://discord.gg/gCyBu8T