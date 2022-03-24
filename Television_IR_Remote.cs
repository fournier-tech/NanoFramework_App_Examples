using System;
using nanoFramework.Hardware.Esp32;
using nanoFramework.Hardware.Esp32.Rmt;
//using nanoFramework.Runtime.Native;
using System.Device.Gpio;

namespace NanoFramework_App_Examples
{
    internal class Television_IR_Remote //: IDisposable
    {
        //***** nanoFramework.Hardware.Esp32.Rmt == four classes *****
        //RmtChannel -- not instantiated, but inherited by TransmitterChannel
        RmtCommand rmtCommand;
        //ReceiverChannel -- not used in this project
        TransmitterChannel transChannel;

        //***** nanoFramework.Hardware.Esp32 == five classes *****
        //Gpio -- static
        //Configuration -- used to set / get special pin function for communication
        HighResTimer hrTimer = new();
        ulong timeDelay;

        //NativeMemory -- used for Spi comminucation 
        //Sleep sleep;

        //***** setup for button numbering scheme, either logical or board
        //numbering system *****
        private GpioController gpioController = new(PinNumberingScheme.Board);

        //***** fields to declare ESP32 board IO pins ***** 
        GpioPin powerButton;
        GpioPin volumeUpButton;
        GpioPin volumeDownButton;
        GpioPin signalOut;

        ushort duration0;
        bool level0;
        ushort duration1;
        bool level1;
        int buttonCodeBuffer;

        public void RmtConfiguration()
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
            signalOut = gpioController.OpenPin(Gpio.IO04, PinMode.Output);
            transChannel = new TransmitterChannel(signalOut.PinNumber)
            {
                SourceClock = SourceClock.APB,
                ClockDivider = 80,
                CarrierEnabled = true,
                IdleLevel = true,
                IsChannelIdle = true,
                CarrierHighDuration = 0,
                CarrierLowDuration = 0,
                CarrierLevel = false
            };

            //***** configure pin mode for inputs(buttons) *****
            powerButton = gpioController.OpenPin(Gpio.IO36, PinMode.InputPullUp);
            volumeUpButton = gpioController.OpenPin(Gpio.IO35, PinMode.InputPullUp);
            volumeDownButton = gpioController.OpenPin(Gpio.IO34, PinMode.InputPullUp);

            //***** 32 bit address code / command code *****
            //
            //power       == 00011100 11100011 01001000 10110111
            //            == 0x1C     E3       48       B7
            //volume up   == 00011100 11100011 01110000 10001111
            //            == 0x1C     E3       70       8F
            //volume down == 00011100 11100011 11110000 00001111
            //            == 0x1C     E3       F0       0F
            int[] buttonCode = new int[] { 0x1CE348B7, 0x1CE3708F, 0x1CE3F00F };

            while (true)
            {
                if (powerButton.Read() != 1 && volumeUpButton.Read() == 1
                    && volumeDownButton.Read() == 1)
                {
                    buttonCodeBuffer = buttonCode[0];
                    RmtOperation(buttonCodeBuffer);
                }
                else if (volumeUpButton.Read() != 1 && powerButton.Read() == 1
                    && volumeDownButton.Read() == 1)
                {
                    buttonCodeBuffer = buttonCode[1];
                    RmtOperation(buttonCodeBuffer);
                }
                else if (volumeDownButton.Read() != 1 && powerButton.Read() == 1
                    && volumeUpButton.Read() == 1)
                {
                    buttonCodeBuffer = buttonCode[2];
                    RmtOperation(buttonCodeBuffer);
                }
            }
        }
        public void RmtOperation(int buttonCodeBuffer)
        {
            //updating transmitter channel with carrier signal 38.222kHz == 26uS
            //pulse 50% duty cycle
            transChannel.CarrierEnabled = true;
            transChannel.CarrierHighDuration = 13;
            transChannel.CarrierLowDuration = 13;

            BeginCode();

            //***** Iterate through the 32-bit address / button code
            //  used only 29, because the first three bits of the address are zero and is truncated.
            for (int i = 0; i < 32; i++)
            {
                //The MSB is logically anded with 1 if the result
                //is false the LSB is zero otherwise one, then check iterates after a bit shift left 
                if ((buttonCodeBuffer & 0x80000000) == 0)
                {
                    //***** binary zero signal *****
                    duration0 = 562;
                    level0 = false;
                    duration1 = 562;
                    level1 = true;
                    rmtCommand = new(duration0, level0, duration1, level1);

                    transChannel.AddCommand(rmtCommand);
                }
                else
                {
                    //***** binary one signal *****
                    duration0 = 562;
                    level0 = false;
                    duration1 = 1687;
                    level1 = true;
                    rmtCommand = new(duration0, level0, duration1, level1);

                    transChannel.AddCommand(rmtCommand);
                }
                //buttonCodeBuffer >>= 1;
                buttonCodeBuffer <<= 1;
            }

            StopBit();

            //***** if a button is still depressed a single repeat code is sent *****
            if (powerButton.Read() != 1 || volumeUpButton.Read() != 1 || volumeDownButton.Read() != 1)
            {
                timeDelay = 40500;

                hrTimer.StartOneShot(timeDelay);
                hrTimer.OnHighResTimerExpired += Hrs_OnHighResTimerExpired;
            }
            while (powerButton.Read() != 1 || volumeUpButton.Read() != 1 || volumeDownButton.Read() != 1)
            {
                timeDelay = 96187;
                hrTimer.StartOneShot(timeDelay);
                hrTimer.OnHighResTimerExpired += Hrs_OnHighResTimerExpired;
            }


        }

        private void Hrs_OnHighResTimerExpired(HighResTimer sender, object e)
        {
            RepeatCode();
            StopBit();
        }
        public void BeginCode()
        {

            //***** Begin signal *****
            duration0 = 9000;
            level0 = false;
            duration1 = 4500;
            level1 = true;

            rmtCommand = new(duration0, level0, duration1, level1);
            transChannel.AddCommand(rmtCommand);
        }
        public void RepeatCode()
        {
            //***** Repeat code signal *****
            duration0 = 9000;
            level0 = false;
            duration1 = 2250;
            level1 = true;

            rmtCommand = new(duration0, level0, duration1, level1);
            transChannel.AddCommand(rmtCommand);


        }
        public void StopBit()
        {
            //***** binary zero signal *****
            //***** added at the end, as the code stop bit *****
            duration0 = 562;
            level0 = false;
            duration1 = 562;
            level1 = true;

            rmtCommand = new(duration0, level0, duration1, level1);
            transChannel.AddCommand(rmtCommand);
            transChannel.Send(false);
        }
        //public void garbargeCleanup()
        //{
        //    //GC.Run(GC.Collect());
        //    nanoFramework.Runtime.Native.GC.Run(true);
        //}

        //public void Dispose()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
