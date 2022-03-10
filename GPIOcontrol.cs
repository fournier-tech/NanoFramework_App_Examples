using System;
using System.Device.Gpio;
using System.Threading;

namespace NF_test_app_2022
{
    internal class GPIOcontrol
    {
        private static GpioController s_GpioController;
        public void BlinkLed()
        {

            //Thread.Sleep(Timeout.Infinite);

            s_GpioController = new GpioController();

            // ESP32 DevKit: 4 is a valid GPIO pin in, some boards like Xiuxin ESP32 may require GPIO Pin 2 instead.
            GpioPin led = s_GpioController.OpenPin(2, PinMode.Output);

            //STM32F769I_DISCO: PJ5 is LD2
            //GpioPin led = s_GpioController.OpenPin(PinNumber('J', 5), PinMode.Output);

            led.Write(PinValue.Low);


            while (true)
            {
                led.Toggle();
                Thread.Sleep(125);
                led.Toggle();
                Thread.Sleep(225);
                led.Toggle();
                Thread.Sleep(325);
                led.Toggle();
                Thread.Sleep(425);
            }
        }
        //static int PinNumber(char port, byte pin)
        //{
        //    if (port < 'A' || port > 'J')
        //        throw new ArgumentException();

        //    return ((port - 'A') * 16) + pin;
        //}
    }
}
