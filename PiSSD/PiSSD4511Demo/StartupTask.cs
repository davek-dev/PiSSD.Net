using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using PiSSD._4511;
using System.Threading.Tasks;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace PiSSD4511Demo
{
    public sealed class StartupTask : IBackgroundTask
    {
        // <summary>
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

            this.Run3DigitDisplay();
        }

        private void Run3DigitDisplay()
        {
            // Set the pin numbers
            Display display = new Display(5, 6, 19, 26, 23, 25, 12);

            // Turn on the display
            display.EnableDisplay();

            // Run a test
            //display.DisplayTest();

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
    }
}
