using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using System.Threading.Tasks;
using PiSSD;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace PiSSDDemo
{
    public sealed class StartupTask : IBackgroundTask
    {
        /// <summary>
        /// The deferral
        /// </summary>
        BackgroundTaskDeferral deferral;

        /// <summary>
        /// Runs the specified task instance.
        /// </summary>
        /// <param name="taskInstance">The task instance.</param>
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();

            //this.Run4DigitDisplay();

            this.Run2DigitDisplay();
        }

        private void Run4DigitDisplay()
        {
            // Set the pin numbers
            Display display = new Display(26, 19, 13, 6, 5, 22, 4, 25, 24, 23, 18);

            // Turn on the display
            display.EnableDisplay();

            // Run a test
            display.DisplayTest();

            // Display some numbers

            for (int i = 0; i < 100; i++)
            {
                display.DisplayNumber(i, true);
                Task.Delay(100).Wait();
            }

            for (int i = 100; i > 0; i--)
            {
                display.DisplayNumber(i, false);
                Task.Delay(100).Wait();
            }

            // Turn off the display
            display.DisableDisplay();
        }

        private void Run2DigitDisplay()
        {           
            // Set the pin numbers
            Display display = new Display(26, 19, 13, 6, 5, 22, 4, 25, 24);

            // Turn on the display
            display.EnableDisplay();

            // Run a test
            display.DisplayTest();

            // Display some numbers

            for (int i = 0; i < 99; i++)
            {
                display.DisplayNumber(i, true);
                Task.Delay(100).Wait();
            }

            for (int i = 99; i > 0; i--)
            {
                display.DisplayNumber(i, false);
                Task.Delay(100).Wait();
            }

            // Turn off the display
            display.DisableDisplay();
        }

    }
}
