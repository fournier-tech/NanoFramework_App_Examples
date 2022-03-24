//using System;
using nanoFramework.Hardware.Esp32;
using System.Device.Gpio;
using System.Threading;

namespace NanoFramework_App_Examples
{
    internal class GPIOcontrol
    {
        private static GpioController s_GpioController;
        public void BlinkLed()
        {
            //Thread.Sleep(Timeout.Infinite);

            s_GpioController = new GpioController();

            // ESP32 DevKit: 4 is a valid GPIO pin in, some boards like Xiuxin ESP32 may require GPIO Pin 2 instead.
            GpioPin led = s_GpioController.OpenPin(Gpio.IO02, PinMode.Output);

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



        //****** Dev Kit Pinout (#1 location top left count counter-clockwise) ******
        //GPIO = general purpose input/output
        //ADC = Analog-to-Digital Converter
        //RTC = real-time clock
        //Touch = capacitive touch sensors

        //SD card
        //UART
        //SPI
        //SDIO
        //I2C
        //LED PWM_Test
        //Motor PWM_Test
        //I2S
        //IR
        //pulse counter
        //DAC
        //TWAI = two-wire automotive interface

        //Pin  1 -- Gnd
        //Pin  2 -- 3.3V
        //Pin  3 -- Enable / Reset
        //Pin  4 -- GPIO 36 / ADC1_CH0 / RTC_GPIO0
        //Pin  5 -- GPIO 39 / ADC1_CH3 / RTC_GPIO3
        //Pin  6 -- GPIO 34 / ADC1_CH6 / RTC_GPIO4
        //Pin  7 -- GPIO 35 / ADC1_CH7 / RTC_GPIO5 
        //Pin  8 -- GPIO 32 / ADC1_CH4 / Touch 9 / XTAL_32K_P (32.768 kHz crystal oscillator input) / RTC_GPIO9
        //Pin  9 -- GPIO 33 / ADC1_CH5 / Touch 8
        //Pin 10 -- GPIO 25 / ADC1_CH18 
        //Pin 11 -- GPIO 26 / ADC1_CH19
        //Pin 12 -- GPIO 27 / ADC1_17 / Touch 7
        //Pin 13 -- GPIO 14 / ADC1_16 / Touch 6
        //Pin 14 -- GPIO 12 / ADC1_15 / Touch 5
        //Pin 15 -- Gnd
        //Pin 16 -- GPIO 13 / ADC1_14 / Touch 4
        //Pin 17 -- GPIO 9 / SD 2
        //Pin 18 -- GPIO 10 / SD 3
        //Pin 19 -- GPIO 11 / CMD

        //Pin 20 -- GPIO 6 / CLK
        //Pin 21 -- GPIO 7 / SD 0
        //Pin 22 -- GPIO 8 / SD 1
        //Pin 23 -- GPIO 15 / ADC1_13 / Touch 3
        //Pin 24 -- GPIO 2 / ADC1_12 / Touch 2
        //Pin 25 -- GPIO 0 / ADC1_11 / Touch 1 / CLK_OUT1 / EMAC_TX_CLK
        //Pin 26 -- GPIO 4 / ADC1_10 / Touch 0
        //Pin 27 -- GPIO 16
        //Pin 28 -- GPIO 17
        //Pin 29 -- GPIO 5 / VSPI CS 0 / HS1_DATA 6 / EMAC_RX_CLK
        //Pin 30 -- GPIO 8 / VSPI CLK / HS1_DATA 7
        //Pin 31 -- GPIO 19 / VSPI MISO
        //Pin 32 -- N.C.
        //Pin 33 -- GPIO 21
        //Pin 34 -- GPIO 3 / RXD 0
        //Pin 35 -- GPIO 1 / TXD 0
        //Pin 36 -- GPIO 22
        //Pin 37 -- GPIO 23 / VSPI MOSI
        //Pin 38 -- Gnd
        //Pin 39 -- Gnd (Pad)
    }
}
