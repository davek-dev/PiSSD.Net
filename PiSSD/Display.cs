using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace PiSSD
{
    public class Display
    {
        CancellationTokenSource cts;
        CancellationToken token;

        private GpioPin PinSegA;
        private GpioPin PinSegB;
        private GpioPin PinSegC;
        private GpioPin PinSegD;
        private GpioPin PinSegE;
        private GpioPin PinSegF;
        private GpioPin PinSegG;

        private GpioPin[] Displays;
        private int[] DisplayDigits;

        private GpioPin PinDP;

        private int displayNo = 0;
        private bool displayLeadingZero;
        private bool running = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Display"/> class.
        /// </summary>
        /// <param name="segA">The seg a.</param>
        /// <param name="segB">The seg b.</param>
        /// <param name="segC">The seg c.</param>
        /// <param name="segD">The seg d.</param>
        /// <param name="segE">The seg e.</param>
        /// <param name="segF">The seg f.</param>
        /// <param name="segG">The seg g.</param>
        /// <param name="displayPins">The displays pin [one for each display digit].</param>
        public Display(int segA, int segB, int segC, int segD, int segE, int segF, int segG, params int[] displayPins)
        {
            this.Displays = new GpioPin[displayPins.Length];

            for (int i = 0; i < displayPins.Length; i++)
            {
                GpioPin pin = GpioController.GetDefault().OpenPin(displayPins[i]);
                pin.Write(GpioPinValue.High);
                pin.SetDriveMode(GpioPinDriveMode.Output);
                this.Displays[i] = pin;
            }

            this.SetupOutputPin(ref this.PinSegA, segA);
            this.SetupOutputPin(ref this.PinSegB, segB);
            this.SetupOutputPin(ref this.PinSegC, segC);
            this.SetupOutputPin(ref this.PinSegD, segD);
            this.SetupOutputPin(ref this.PinSegE, segE);
            this.SetupOutputPin(ref this.PinSegF, segF);
            this.SetupOutputPin(ref this.PinSegG, segG);

            this.cts = new CancellationTokenSource();
            this.token = new CancellationToken();
        }

        /// <summary>
        /// Setup the GPIO pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="pinNo">The pin no.</param>
        private void SetupOutputPin(ref GpioPin pin, int pinNo)
        {
            pin = GpioController.GetDefault().OpenPin(pinNo);
            pin.Write(GpioPinValue.High);
            pin.SetDriveMode(GpioPinDriveMode.Output);
        }

        /// <summary>
        /// Sets the pins high.
        /// </summary>
        /// <param name="pins">The pins.</param>
        private void SetHigh(GpioPin[] pins)
        {
            foreach (GpioPin p in pins)
            {
                p.Write(GpioPinValue.High);
            }
        }

        /// <summary>
        /// Sets the pins low.
        /// </summary>
        /// <param name="pins">The pins.</param>
        private void SetLow(GpioPin[] pins)
        {
            foreach (GpioPin p in pins)
            {
                p.Write(GpioPinValue.Low);
            }
        }

        /// <summary>
        /// Clears the display.
        /// </summary>
        private void ClearDisplay()
        {
            this.SetHigh(this.Displays);
        }

        /// <summary>
        /// Sets the specified display to the specified value.
        /// </summary>
        /// <param name="displayPin">The display pin.</param>
        /// <param name="value">The value.</param>
        private void SetDisplay(GpioPin displayPin, int value)
        {
            this.ClearDisplay();

            switch (value)
            {
                case 0:
                    this.SetHigh(new GpioPin[] { this.PinSegA, this.PinSegB, this.PinSegC, this.PinSegD, this.PinSegE, this.PinSegF });
                    this.SetLow(new GpioPin[] { this.PinSegG });
                    break;
                case 1:
                    this.SetHigh(new GpioPin[] { this.PinSegB, this.PinSegC });
                    this.SetLow(new GpioPin[] { this.PinSegA, this.PinSegD, this.PinSegE, this.PinSegF, this.PinSegG });
                    break;
                case 2:
                    this.SetHigh(new GpioPin[] { this.PinSegA, this.PinSegB, this.PinSegD, this.PinSegE, this.PinSegG });
                    this.SetLow(new GpioPin[] { this.PinSegC, this.PinSegF });
                    break;
                case 3:
                    this.SetHigh(new GpioPin[] { this.PinSegA, this.PinSegB, this.PinSegC, this.PinSegD, this.PinSegG });
                    this.SetLow(new GpioPin[] { this.PinSegE, this.PinSegF });
                    break;
                case 4:
                    this.SetHigh(new GpioPin[] { this.PinSegB, this.PinSegC, this.PinSegF, this.PinSegG });
                    this.SetLow(new GpioPin[] { this.PinSegA, this.PinSegD, this.PinSegE });
                    break;
                case 5:
                    this.SetHigh(new GpioPin[] { this.PinSegA, this.PinSegC, this.PinSegD, this.PinSegF, this.PinSegG });
                    this.SetLow(new GpioPin[] { this.PinSegB, this.PinSegE });
                    break;
                case 6:
                    this.SetHigh(new GpioPin[] { this.PinSegA, this.PinSegC, this.PinSegD, this.PinSegE, this.PinSegF, this.PinSegG });
                    this.SetLow(new GpioPin[] { this.PinSegB });
                    break;
                case 7:
                    this.SetHigh(new GpioPin[] { this.PinSegA, this.PinSegB, this.PinSegC });
                    this.SetLow(new GpioPin[] { this.PinSegD, this.PinSegE, this.PinSegF, this.PinSegG });
                    break;
                case 8:
                    this.SetHigh(new GpioPin[] { this.PinSegA, this.PinSegB, this.PinSegC, this.PinSegD, this.PinSegE, this.PinSegF, this.PinSegG });
                    break;
                case 9:
                    this.SetHigh(new GpioPin[] { this.PinSegA, this.PinSegB, this.PinSegC, this.PinSegD, this.PinSegF, this.PinSegG });
                    this.SetLow(new GpioPin[] { this.PinSegE });
                    break;
                case 10:  // Clear Display
                    this.SetLow(new GpioPin[] { this.PinSegA, this.PinSegB, this.PinSegC, this.PinSegD, this.PinSegE, this.PinSegF, this.PinSegG });
                    break;
                default:
                    this.SetLow(new GpioPin[] { this.PinSegA, this.PinSegB, this.PinSegC, this.PinSegD, this.PinSegE, this.PinSegF, this.PinSegG });
                    break;
            }

            this.SetLow(new GpioPin[] { displayPin });
        }

        /// <summary>
        /// Display all segments for 10 seconds
        /// </summary>
        public void DisplayTest()
        {
            string testStr = "";

            foreach(GpioPin d in this.Displays)
            {
                testStr = testStr + "8";
            }
             if(testStr.Length < 1)
            {
                return;
            }

            int output = int.Parse(testStr);

            this.DisplayNumber(output);
            Task.Delay(10000).Wait();
        }

        /// <summary>
        /// Displays the number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="displayLeadingZero">if set to <c>true</c> [display leading zeros].</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Number cannot be greater than 9999
        /// or
        /// Number cannot be negative
        /// </exception>
        public void DisplayNumber(int number, bool displayLeadingZero = true)
        {
            this.displayNo = number;
            this.displayLeadingZero = displayLeadingZero;

            if (this.displayNo < 0)
            {
                throw new ArgumentOutOfRangeException("Number cannot be negative");
            }

            int checkMax = 1;
            for(int i = 0; i < this.DisplayDigits.Length; i++)
            {
                checkMax = checkMax * 10;
            }

            if(number >= checkMax)
            {
                throw new ArgumentException("Cannot display numbers greater than " + (checkMax - 1).ToString());
            }

            if (this.displayNo == 0)
            {
                this.Blank();
                if(this.DisplayDigits.Length > 0)
                {
                    this.DisplayDigits[0] = 0;
                }
            }
            else
            {
                List<int> listOfInts = new List<int>();
                while (this.displayNo > 0)
                {
                    listOfInts.Add(this.displayNo % 10);
                    this.displayNo = this.displayNo / 10;
                }

                if (displayLeadingZero)
                {
                    while (listOfInts.Count < this.Displays.Length)
                    {
                        listOfInts.Add(0);
                    }
                }
                else
                {
                    while (listOfInts.Count < this.Displays.Length)
                    {
                        listOfInts.Add(10);
                    }
                }

                this.DisplayDigits = listOfInts.ToArray();
            }
        }

        /// <summary>
        /// Enables the display.
        /// </summary>
        public void EnableDisplay()
        {
            this.Start();
        }

        /// <summary>
        /// Disables the display.
        /// </summary>
        public void DisableDisplay()
        {
            this.cts.Cancel();
            this.ClearDisplay();
            this.running = false;
        }

        /// <summary>
        /// Starts the screen display task.
        /// </summary>
        private void Start()
        {
            if (running)
            {
                return;
            }

            running = true;

            Task.Factory.StartNew(() =>
            {
                while (!this.cts.IsCancellationRequested)
                {
                    if (this.DisplayDigits == null)
                    {
                        this.Blank();
                    }

                    int[] arrDigs = this.DisplayDigits;

                    for (int i = 0; i < arrDigs.Length; i++)
                    {
                        this.SetDisplay(this.Displays[i], arrDigs[i]);
                    }
                }
            }, token);
        }

        private void Blank()
        {
            this.DisplayDigits = new int[this.Displays.Length];

            for (int i = 0; i < this.Displays.Length; i++)
            {
                this.DisplayDigits[i] = 10;
            }
        }
    }
}
