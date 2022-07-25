namespace NanoFramework_App_Examples
{
    public class Program
    {
        public static void Main()
        {
            Television_IR_Remote tvRemote = new();
            tvRemote.RmtConfiguration();
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