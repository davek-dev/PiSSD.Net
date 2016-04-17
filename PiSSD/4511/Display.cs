using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Devices.Gpio;

/// <summary>
/// Extracted from base display to power a 7 segment display through a 4511 Integrated Circuit
/// </summary>
namespace PiSSD._4511
{
    public class Display : PiSSD.Display
    {
        private GpioPin pinBcd0;
        private GpioPin pinBcd1;
        private GpioPin pinBcd2;
        private GpioPin pinBcd3;

        public Display(int bcd0, int bcd1, int bcd2, int bcd3, params int[] displayPins) : base ()
        {
            this.Displays = new GpioPin[displayPins.Length];

            for (int i = 0; i < displayPins.Length; i++)
            {
                GpioPin pin = GpioController.GetDefault().OpenPin(displayPins[i]);
                pin.Write(GpioPinValue.Low);
                pin.SetDriveMode(GpioPinDriveMode.Output);
                this.Displays[i] = pin;
            }

            this.SetupOutputPin(ref this.pinBcd0, bcd0);
            this.SetupOutputPin(ref this.pinBcd1, bcd1);
            this.SetupOutputPin(ref this.pinBcd2, bcd2);
            this.SetupOutputPin(ref this.pinBcd3, bcd3);
        }

        protected override void SetDisplay(GpioPin displayPin, int value)
        {
            this.ClearDisplay();

            switch (value)
            {
                case 0:
                    this.SetLow(new GpioPin[] { this.pinBcd0, this.pinBcd1, this.pinBcd2, this.pinBcd3 });
                    break;
                case 1:
                    this.SetHigh(new GpioPin[] { this.pinBcd0 });
                    this.SetLow(new GpioPin[] { this.pinBcd1, this.pinBcd2, this.pinBcd3 });
                    break;
                case 2:
                    this.SetHigh(new GpioPin[] { this.pinBcd1 });
                    this.SetLow(new GpioPin[] { this.pinBcd0, this.pinBcd2, this.pinBcd3 });
                    break;
                case 3:
                    this.SetHigh(new GpioPin[] { this.pinBcd0, this.pinBcd1 });
                    this.SetLow(new GpioPin[] { this.pinBcd2, this.pinBcd3 });
                    break;
                case 4:
                    this.SetHigh(new GpioPin[] { this.pinBcd2 });
                    this.SetLow(new GpioPin[] { this.pinBcd0, this.pinBcd1, this.pinBcd3 });
                    break;
                case 5:
                    this.SetHigh(new GpioPin[] { this.pinBcd0, this.pinBcd2 });
                    this.SetLow(new GpioPin[] { this.pinBcd1,this.pinBcd3 });
                    break;
                case 6:
                    this.SetHigh(new GpioPin[] { this.pinBcd1, this.pinBcd2});
                    this.SetLow(new GpioPin[] { this.pinBcd0, this.pinBcd3 });
                    break;
                case 7:
                    this.SetHigh(new GpioPin[] { this.pinBcd0, this.pinBcd1, this.pinBcd2 });
                    this.SetLow(new GpioPin[] { this.pinBcd3 });
                    break;
                case 8:
                    this.SetHigh(new GpioPin[] { this.pinBcd3 });
                    this.SetLow(new GpioPin[] { this.pinBcd0, this.pinBcd1, this.pinBcd2 });
                    break;
                case 9:
                    this.SetHigh(new GpioPin[] { this.pinBcd0, this.pinBcd3 });
                    this.SetLow(new GpioPin[] { this.pinBcd1, this.pinBcd2 });
                    break;
                case 10:  // Clear Display
                    this.SetHigh(new GpioPin[] { this.pinBcd0, this.pinBcd1, this.pinBcd2, this.pinBcd3 });
                    break;
                default:
                    this.SetHigh(new GpioPin[] { this.pinBcd0, this.pinBcd1, this.pinBcd2, this.pinBcd3 });
                    break;
            }

            this.SetHigh(new GpioPin[] { displayPin });
        }

        protected override void ClearDisplay()
        {
            this.SetLow(this.Displays);
        }
    }
}
