using nanoFramework.Hardware.Esp32;
using nanoFramework.Hardware.Esp32.Rmt;
using System.Device.Gpio;

namespace NanoFramework_App_Examples
{
    internal class Television_IR_Remote
    {
        //***** nanoFramework.Hardware.Esp32.Rmt == four classes *****
        //***** nanoFramework.Hardware.Esp32 == five classes *****

        RmtCommand rmtCommand;
        TransmitterChannel transChannel;

        //***** 32 bit address code / command code *****
        //
        //power       == 00011100 11100011 01001000 10110111
        //            == 0x1C     E3       48       B7
        //volume up   == 00011100 11100011 01110000 10001111
        //            == 0x1C     E3       70       8F
        //volume down == 00011100 11100011 11110000 00001111
        //            == 0x1C     E3       F0       0F
        int[] buttonCode = new int[] { 0x1CE348B7, 0x1CE3708F, 0x1CE3F00F };
        int buttonCodeBuffer;
        ulong timeDelay;

        //***** setup for button numbering scheme, either logical or board
        //numbering system *****
        GpioController gpioController = new(PinNumberingScheme.Board);

        //***** fields to declare ESP32 board IO pins ***** 
        GpioPin powerButton;
        GpioPin volumeUpButton;
        GpioPin volumeDownButton;
        GpioPin signalOut;

        //***** required fields to instanciate the RmtCommand class *****
        ushort duration0;
        bool level0;
        ushort duration1;
        bool level1;

        public void RmtConfiguration()
        {
            ConfigurePins();
            ConfigureTransmitter();
            
            while (true)
            {
                HighResTimer contRepeatHighResTimer = new();
                if (powerButton.Read() != 1 && volumeUpButton.Read() == 1 && volumeDownButton.Read() == 1)
                {
                    buttonCodeBuffer = buttonCode[0];
                    RmtOperation();
                    timeDelay = 1000000000;
                    contRepeatHighResTimer.StartOneShot(timeDelay);
                    //ConfigureTransmitter();
                }
                else if (volumeUpButton.Read() != 1 && powerButton.Read() == 1 && volumeDownButton.Read() == 1)
                {
                    buttonCodeBuffer = buttonCode[1];
                    RmtOperation();
                    timeDelay = 1000000000;
                    contRepeatHighResTimer.StartOneShot(timeDelay);
                    //ConfigureTransmitter();
                }
                else if (volumeDownButton.Read() != 1 && powerButton.Read() == 1 && volumeUpButton.Read() == 1)
                {
                    buttonCodeBuffer = buttonCode[2];
                    RmtOperation();
                    timeDelay = 1000000000;
                    contRepeatHighResTimer.StartOneShot(timeDelay);
                    //ConfigureTransmitter();
                }
                
                //int test = transChannel[a]
                contRepeatHighResTimer.Dispose();
                //if (transChannel.Channel. >= 500) { 
                //}
            }
        }
        public void ConfigurePins()
        {
            //***** configure pin mode for inputs(buttons) *****
            powerButton = gpioController.OpenPin(Gpio.IO36, PinMode.InputPullUp);
            volumeUpButton = gpioController.OpenPin(Gpio.IO35, PinMode.InputPullUp);
            volumeDownButton = gpioController.OpenPin(Gpio.IO34, PinMode.InputPullUp);
            signalOut = gpioController.OpenPin(Gpio.IO04, PinMode.Output);
            //signalOut.Write(false);
        }
        public void RmtOperation()
        {
            BeginCode();
            FunctionCode();

            //***** if a button is still depressed a single RepeatCode() function call is sent *****
            //***** the first repeat code is 108mS from lead edge of BeginCode()
            //***** all remaining repeat codes are a 108mS from the lead edge of the prior repeat code
            if (powerButton.Read() != 1 || volumeUpButton.Read() != 1 || volumeDownButton.Read() != 1)
            {
                for (int i = 0; i < 72; i++)
                {
                    //72 * 562.5uS = 40500uS == 40.500mS
                    duration0 = 0;
                    level0 = true;
                    duration1 = 562;
                    level1 = false;

                    rmtCommand = new(duration0, level0, duration1, level1);
                    transChannel.AddCommand(rmtCommand);
                }
                RepeatCode();
            }
            while (powerButton.Read() != 1 || volumeUpButton.Read() != 1 || volumeDownButton.Read() != 1)
            {
                for (int i = 0; i < 171; i++)
                {
                    //171 * 562.5uS = 96187uS == 96.187mS
                    duration0 = 0;
                    level0 = true;
                    duration1 = 562;
                    level1 = false;

                    rmtCommand = new(duration0, level0, duration1, level1);
                    transChannel.AddCommand(rmtCommand);
                }
                RepeatCode();
            }
        }
        public void ConfigureTransmitter()
        {
            //*****  Setup for the TransmitterChannel *****
            //  The clock source is 80MHz -- ClockDivider(80) == 1MHz clock
            //  ticks == 1uS time.
            //  IdleLevel == Is similar to the CarrierLevel signal, but in an idle state.
            //  CarrierHighDuration == The number of clock cycles for a high signal.
            //  CarrierLowDuration == Is similiar to a duty cycle - the number of clock
            //  cycles for a low signal.
            //  CarrierLevel == for a high output(true -- low on one high on zero)
            //  or low output(false -- high on one low on zero) frequency.

            transChannel = new TransmitterChannel(signalOut.PinNumber)
            {
                SourceClock = SourceClock.APB,
                ClockDivider = 80,
                CarrierEnabled = true,
                IdleLevel = false,
                IsChannelIdle = true,
                //updating transmitter channel with carrier signal 38.222kHz == 26uS
                //pulse 50% duty cycle
                CarrierHighDuration = 13,
                CarrierLowDuration = 13,
                CarrierLevel = true
            };
        }
        public void BeginCode()
        {
            //***** Begin signal *****
            duration0 = 9000;
            level0 = true;
            duration1 = 4500;
            level1 = false;

            rmtCommand = new(duration0, level0, duration1, level1);
            transChannel.AddCommand(rmtCommand);
        }
        public void FunctionCode()
        {
            //***** Iterate through the 32-bit address / button code
            for (int i = 0; i < 32; i++)
            {
                //The MSB is logically anded with 1 if the result
                //is false the LSB is zero otherwise one, then check iterates after a bit shift left 
                if ((buttonCodeBuffer & 0x80000000) == 0)
                {
                    //***** binary zero signal *****
                    duration0 = 563;
                    level0 = true;
                    duration1 = 562;
                    level1 = false;

                    rmtCommand = new(duration0, level0, duration1, level1);
                    transChannel.AddCommand(rmtCommand);
                }
                else
                {
                    //***** binary one signal *****
                    duration0 = 563;
                    level0 = true;
                    duration1 = 1687;
                    level1 = false;

                    rmtCommand = new(duration0, level0, duration1, level1);
                    transChannel.AddCommand(rmtCommand);
                }
                buttonCodeBuffer <<= 1;
            }
            StopBit();
        }
        public void StopBit()
        {
            //***** binary zero signal added at the end, as the code stop bit, then message send to receiver *****
            //duration1 = 562;
            duration0 = 563;
            level0 = true;
            duration1 = 20;
            level1 = false;

            rmtCommand = new(duration0, level0, duration1, level1);
            transChannel.AddCommand(rmtCommand);

            transChannel.Send(true);
        }
        public void RepeatCode()
        {
            //***** Repeat code signal *****
            duration0 = 9000;
            level0 = true;
            duration1 = 2250;
            level1 = false;

            rmtCommand = new(duration0, level0, duration1, level1);
            transChannel.AddCommand(rmtCommand);

            StopBit();
        }
    }
}