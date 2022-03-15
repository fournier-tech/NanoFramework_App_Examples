//using System;
using nanoFramework.Hardware.Esp32.Rmt;
using nanoFramework.Hardware.Esp32;
//using System.Text;

namespace NanoFramework_App_Examples
{
    internal class Television_IR_Remote
    {
        //***** nanoFramework.Hardware.Esp32.Rmt == four classes
        //ReceiverChannel
        RmtChannel rmt_ch;
        RmtCommand rmt_cmd;
        TransmitterChannel trans_ch;

        //***** nanoFramework.Hardware.Esp32 == five classes
        //***** classes:
        //Gpio -- static
        Configuration config;
        HighResTimer hr_timer;
        NativeMemory n_memory;
        Sleep sleep;

        public void rmt_configuration()
        {
            //***** not using receiver for this project
            //ReceiverChannel rec_ch = new ReceiverChannel();
            rmt_ch = new();

            rmt_cmd.Duration0 = 255;
            rmt_cmd.Level0 = false;
            rmt_cmd.Duration1 = 255;
            rmt_cmd.Level1 = true;
            rmt_cmd = new RmtCommand(rmt_cmd.Duration0, rmt_cmd.Level0, rmt_cmd.Duration1, rmt_cmd.Level1);

            trans_ch = new TransmitterChannel(Gpio.IO14);
            config = new Configuration();
            hr_timer = new HighResTimer();
            n_memory = new NativeMemory();
            sleep = new Sleep();

        }


        TransmitterChannel tc = new TransmitterChannel(Gpio.IO10);
        RmtCommand beginTransmission = new RmtCommand(9000, true, 4500, false);
        RmtCommand zeroTransmission = new RmtCommand(560, true, 2250, false);
        RmtCommand oneTransmission = new RmtCommand(560, true, 560, false);
        RmtCommand repeatTransmission = new RmtCommand(9000, true, 2250, false);
        public void TimerPause()
        {

            tc.CarrierLevel = true;
            tc.CarrierHighDuration = 2250;
            tc.CarrierLowDuration = 1125;
            tc.CarrierEnabled = true;

            tc.AddCommand(beginTransmission);
            tc.AddCommand(zeroTransmission);
            tc.AddCommand(oneTransmission);
            tc.AddCommand(repeatTransmission);
        }
    }
}
