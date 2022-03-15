using System.Device.Pwm;
using nanoFramework.Hardware.Esp32;

namespace NanoFramework_App_Examples
{
    internal class PWM_Test
    {
        public void PwmOperation()
        {
            //PWM1 - PWM8 are low resolution, PWM9 - PWM16 are high resolution
            Configuration.SetPinFunction(Gpio.IO02, DeviceFunction.PWM1);


            PwmChannel pwmPin = PwmChannel.CreateFromPin(Gpio.IO02, 1000, 0.0);
            pwmPin.Start();
            double dutyCycle = 0.0;

            while (true)
            {
                while (dutyCycle < 1.0)
                {
                    pwmPin = PwmChannel.CreateFromPin(Gpio.IO02, 10000, dutyCycle);
                    dutyCycle += 0.0005;
                }
                dutyCycle = 1.0;

                while (dutyCycle > 0.0)
                {
                    pwmPin = PwmChannel.CreateFromPin(Gpio.IO02, 10000, dutyCycle);
                    dutyCycle -= 0.0005;
                }
                dutyCycle = 0.0;
            }
        }
    }
}
/*associated website material
 
 * https://www.kernel.org/doc/html/v5.14-rc6/userspace-api/media/rc/rc-protos.html
  
 * https://www.petervis.com/Raspberry_PI/Driving_LEDs_with_CMOS_and_TTL_Outputs/Driving_LEDs_with_CMOS_and_TTL_Outputs.html
 
 * https://www.sbprojects.net/knowledge/ir/index.php
 
 * https://www.kernel.org/doc/html/v5.14-rc6/userspace-api/media/intro.html
 
 * http://www.diegm.uniud.it/bernardini/Laboratorio_Didattico/2016-2017/2017-Telecomando/ir-protocols.html#:~:text=In%20general%2C%20an%20IR%20Protocol,for%20another%20amount%20of%20time.
 
 * https://www.engineersgarage.com/tv-remote-hack-using-arduino-and-ir-sensor/
 
 * https://www.hackster.io/techmirtz/finding-the-ir-codes-of-any-ir-remote-using-arduino-c7a852
 
 * https://sites.ntc.doe.gov/partners/tr/Training%20Textbooks/08-Engineering%20Symbology,%20Prints,%20and%20Drawings/4-Mod%204-Electronic%20Diagrams%20and%20Schematics.pdf
 */
