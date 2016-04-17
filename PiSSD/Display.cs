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

        private GpioPin pinSegA;
        private GpioPin pinSegB;
        private GpioPin pinSegC;
        private GpioPin pinSegD;
        private GpioPin pinSegE;
        private GpioPin pinSegF;
        private GpioPin pinSegG;

        private GpioPin[] displays;
        private int[] displayDigits;

        private GpioPin pinDP;

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
            this.displays = new GpioPin[displayPins.Length];

            for (int i = 0; i < displayPins.Length; i++)
            {
                GpioPin pin = GpioController.GetDefault().OpenPin(displayPins[i]);
                pin.Write(GpioPinValue.High);
                pin.SetDriveMode(GpioPinDriveMode.Output);
                this.displays[i] = pin;
            }

            this.SetupOutputPin(ref this.pinSegA, segA);
            this.SetupOutputPin(ref this.pinSegB, segB);
            this.SetupOutputPin(ref this.pinSegC, segC);
            this.SetupOutputPin(ref this.pinSegD, segD);
            this.SetupOutputPin(ref this.pinSegE, segE);
            this.SetupOutputPin(ref this.pinSegF, segF);
            this.SetupOutputPin(ref this.pinSegG, segG);

            this.cts = new CancellationTokenSource();
            this.token = new CancellationToken();
        }

        /// <summary>
        /// Setup the GPIO pin.
        /// </summary>
        /// <param name="pin">The pin.</param>
        /// <param name="pinNo">The pin no.</param>
        protected void SetupOutputPin(ref GpioPin pin, int pinNo)
        {
            pin = GpioController.GetDefault().OpenPin(pinNo);
            pin.Write(GpioPinValue.High);
            pin.SetDriveMode(GpioPinDriveMode.Output);
        }

        /// <summary>
        /// Sets the pins high.
        /// </summary>
        /// <param name="pins">The pins.</param>
        protected void SetHigh(GpioPin[] pins)
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
        protected void SetLow(GpioPin[] pins)
        {
            foreach (GpioPin p in pins)
            {
                p.Write(GpioPinValue.Low);
            }
        }

        /// <summary>
        /// Clears the display.
        /// </summary>
        protected virtual void ClearDisplay()
        {
            this.SetHigh(this.displays);
        }

        /// <summary>
        /// Sets the specified display to the specified value.
        /// </summary>
        /// <param name="displayPin">The display pin.</param>
        /// <param name="value">The value.</param>
        protected virtual void SetDisplay(GpioPin displayPin, int value)
        {
            this.ClearDisplay();

            switch (value)
            {
                case 0:
                    this.SetHigh(new GpioPin[] { this.pinSegA, this.pinSegB, this.pinSegC, this.pinSegD, this.pinSegE, this.pinSegF });
                    this.SetLow(new GpioPin[] { this.pinSegG });
                    break;
                case 1:
                    this.SetHigh(new GpioPin[] { this.pinSegB, this.pinSegC });
                    this.SetLow(new GpioPin[] { this.pinSegA, this.pinSegD, this.pinSegE, this.pinSegF, this.pinSegG });
                    break;
                case 2:
                    this.SetHigh(new GpioPin[] { this.pinSegA, this.pinSegB, this.pinSegD, this.pinSegE, this.pinSegG });
                    this.SetLow(new GpioPin[] { this.pinSegC, this.pinSegF });
                    break;
                case 3:
                    this.SetHigh(new GpioPin[] { this.pinSegA, this.pinSegB, this.pinSegC, this.pinSegD, this.pinSegG });
                    this.SetLow(new GpioPin[] { this.pinSegE, this.pinSegF });
                    break;
                case 4:
                    this.SetHigh(new GpioPin[] { this.pinSegB, this.pinSegC, this.pinSegF, this.pinSegG });
                    this.SetLow(new GpioPin[] { this.pinSegA, this.pinSegD, this.pinSegE });
                    break;
                case 5:
                    this.SetHigh(new GpioPin[] { this.pinSegA, this.pinSegC, this.pinSegD, this.pinSegF, this.pinSegG });
                    this.SetLow(new GpioPin[] { this.pinSegB, this.pinSegE });
                    break;
                case 6:
                    this.SetHigh(new GpioPin[] { this.pinSegA, this.pinSegC, this.pinSegD, this.pinSegE, this.pinSegF, this.pinSegG });
                    this.SetLow(new GpioPin[] { this.pinSegB });
                    break;
                case 7:
                    this.SetHigh(new GpioPin[] { this.pinSegA, this.pinSegB, this.pinSegC });
                    this.SetLow(new GpioPin[] { this.pinSegD, this.pinSegE, this.pinSegF, this.pinSegG });
                    break;
                case 8:
                    this.SetHigh(new GpioPin[] { this.pinSegA, this.pinSegB, this.pinSegC, this.pinSegD, this.pinSegE, this.pinSegF, this.pinSegG });
                    break;
                case 9:
                    this.SetHigh(new GpioPin[] { this.pinSegA, this.pinSegB, this.pinSegC, this.pinSegD, this.pinSegF, this.pinSegG });
                    this.SetLow(new GpioPin[] { this.pinSegE });
                    break;
                case 10:  // Clear Display
                    this.SetLow(new GpioPin[] { this.pinSegA, this.pinSegB, this.pinSegC, this.pinSegD, this.pinSegE, this.pinSegF, this.pinSegG });
                    break;
                default:
                    this.SetLow(new GpioPin[] { this.pinSegA, this.pinSegB, this.pinSegC, this.pinSegD, this.pinSegE, this.pinSegF, this.pinSegG });
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

            foreach(GpioPin d in this.displays)
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

            if (this.displayDigits == null)
            {
                this.Blank();
            }

            if (this.displayNo < 0)
            {
                throw new ArgumentOutOfRangeException("Number cannot be negative");
            }

            int checkMax = 1;
            for(int i = 0; i < this.displayDigits.Length; i++)
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
                if(this.displayDigits.Length > 0)
                {
                    this.displayDigits[0] = 0;
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
                    while (listOfInts.Count < this.displays.Length)
                    {
                        listOfInts.Add(0);
                    }
                }
                else
                {
                    while (listOfInts.Count < this.displays.Length)
                    {
                        listOfInts.Add(10);
                    }
                }

                this.displayDigits = listOfInts.ToArray();
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
                    if (this.displayDigits == null)
                    {
                        this.Blank();
                    }

                    int[] arrDigs = this.displayDigits;

                    for (int i = 0; i < arrDigs.Length; i++)
                    {
                        this.SetDisplay(this.displays[i], arrDigs[i]);
                    }
                }
            }, token);
        }

        private void Blank()
        {
            this.displayDigits = new int[this.displays.Length];

            for (int i = 0; i < this.displays.Length; i++)
            {
                this.displayDigits[i] = 10;
            }
        }

        #region Protected Accessors

        protected GpioPin[] Displays
        {
            get
            {
                return this.displays;
            }

            set
            {
                this.displays = value;
            }
        }      

        protected Display()
        {
            this.cts = new CancellationTokenSource();
            this.token = new CancellationToken();
        }

        #endregion
    }
}
