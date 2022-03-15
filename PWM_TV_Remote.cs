//using System;
using System.Device.Gpio;
using System.Device.Pwm;
using System.IO.Ports;
//using System.Diagnostics;
using System.Threading;
using nanoFramework.Hardware.Esp32;

namespace NanoFramework_App_Examples
{
    //https://github.com/nanoframework/Samples/tree/main/samples/SerialCommunication
    internal class PWM_TV_Remote
    {
        //nanoFramework.Hardware.Esp32.Rmt
        //Remote Control Peripheral RMT - designed to work with IR remote carrier signals

        private static GpioController s_GpioController = new();
        private static GpioPin power, volumeUp, volumeDown;
        private static PwmChannel pwmPin;
        SerialPort serialPort;
        public void SerialConfiguration()
        {
            serialPort.BaudRate = 115200;
            serialPort.DataBits = 8;
            serialPort.Parity = Parity.Even;
            serialPort.StopBits = StopBits.One;
            serialPort.WriteBufferSize = 512;
            serialPort.ReceivedBytesThreshold = 120;

            Configuration.SetPinFunction(Gpio.IO09, DeviceFunction.COM2_RX);
            Configuration.SetPinFunction(Gpio.IO10, DeviceFunction.COM2_TX);
            Configuration.SetPinFunction(Gpio.IO06, DeviceFunction.COM2_CTS);
            Configuration.SetPinFunction(Gpio.IO11, DeviceFunction.COM2_RTS);

            //MANDATORY to set pin function to the appropriate COM before instantiating it
            serialPort = new("COM2");

            //myUartConfig.flow_ctrl = UART_HW_FLOWCTRL_DISABLE;
            //myUartConfig.rx_flow_ctrl_thresh = 120;
            serialPort.Open();

            //buffer: The byte array that contains the data to write to the port.
            byte[] writeBuffer = new byte[serialPort.WriteBufferSize];
            //offset: The zero-based byte offset in the buffer parameter at which to begin copying bytes to the port.
            //count: The number of characters to write.
            serialPort.Write(writeBuffer, 0, 4);

        }
        public void RemoteOperation()
        {
            power = s_GpioController.OpenPin(Gpio.IO21, PinMode.Input);
            volumeUp = s_GpioController.OpenPin(Gpio.IO16, PinMode.Input);
            volumeDown = s_GpioController.OpenPin(Gpio.IO17, PinMode.Input);

            PwmSetup();

            while (true)
            {
                if (power.Read() == true)
                {
                    TransmissionStart(1);
                }
                else if (volumeUp.Read() == true)
                {
                    TransmissionStart(2);
                }
                if (volumeDown.Read() == true)
                {
                    TransmissionStart(3);
                }
            }
        }
        public void PwmSetup()
        {
            //PWM1 - PWM8 are low resolution, PWM9 - PWM16 are high resolution
            Configuration.SetPinFunction(Gpio.IO02, DeviceFunction.PWM1);
            pwmPin = PwmChannel.CreateFromPin(Gpio.IO02, 38000, 0.25);

        }
        public void TransmissionStart(int buttonPressed)
        {
            switch (buttonPressed)
            {
                case 1:
                    ButtonPower();
                    break;
                case 2:
                    ButtonVolumeUp();
                    break;
                case 3:
                    ButtonVolumeDown();
                    break;
                default:
                    break;

            }
        }
        public void TransmissionRepeat()
        {

        }
        public void ButtonPower()
        {
            //power = SetPinFunction(Gpio.IO02, DeviceFunction.COM1_TX);

            //SetPinFunction(Gpio.IO02, DeviceFunction.COM1_CTS);
            //SetPinFunction(Gpio.IO03, DeviceFunction.COM1_RTS);
            //SetPinFunction(Gpio.IO04, DeviceFunction.COM1_RX);
            //SetPinFunction(Gpio.IO05, DeviceFunction.COM1_TX);

            //power = s_GpioController.OpenPin(Gpio.IO02, PinMode.Output);
        }
        public void ButtonVolumeUp()
        {

        }
        public void ButtonVolumeDown()
        {

        }

    }
}
