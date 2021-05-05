using System.Threading;
using nanoFramework.Presentation.Media;
using nanoFramework.UI;
using Primitives.SimplePrimitives;
using System.Device.Gpio;
using PuzzleSample;
using System;
using nanoFramework.Presentation;
using TouchInputSample;

namespace Primitives
{
    public class Program
    {
        public static void Main()
        {
            //System.Device.Gpio.GpioController gpioController = new GpioController();
            //GpioPin backLight = gpioController.OpenPin(5, PinMode.Output);
            //backLight.Write(PinValue.High);

            SimpleTouch();
   //         PuzzleProgram();

            Font DisplayFont = Resource.GetFont(Resource.FontResources.SegoeUIRegular12);
            Bitmap fullScreenBitmap = DisplayControl.FullScreen;
            fullScreenBitmap.Clear();

            int delayBetween = 1100;

            // Get full screen bitmap from displayControl to draw on.

            Thread.Sleep(delayBetween);

            while (true)
            {
                RandomDrawLine rdlt = new RandomDrawLine(fullScreenBitmap, DisplayFont);
                Thread.Sleep(delayBetween);

                RotateImage ri = new RotateImage(fullScreenBitmap, DisplayFont);
                Thread.Sleep(delayBetween);

                ColourGradient colourGradient = new ColourGradient(fullScreenBitmap, DisplayFont);
                Thread.Sleep(delayBetween);

                Colours ColourExample = new Colours(fullScreenBitmap, DisplayFont);
                Thread.Sleep(delayBetween);

                PagedText pt = new PagedText(fullScreenBitmap, DisplayFont);
                Thread.Sleep(delayBetween);

                BouncingBalls bb = new BouncingBalls(fullScreenBitmap, DisplayFont);
                Thread.Sleep(delayBetween);

                TileImage ti = new TileImage(fullScreenBitmap, DisplayFont);
                Thread.Sleep(delayBetween);

                StretchImage si = new StretchImage(fullScreenBitmap, DisplayFont);
                Thread.Sleep(delayBetween);

                SetPixels sp = new SetPixels(fullScreenBitmap, DisplayFont);
                Thread.Sleep(delayBetween);

                FontExamples fe = new FontExamples(fullScreenBitmap);
                Thread.Sleep(delayBetween);

                RandomRectangles rr = new RandomRectangles(fullScreenBitmap, DisplayFont);
                Thread.Sleep(delayBetween);

                SliceScaling9 ss = new SliceScaling9(fullScreenBitmap, DisplayFont);
                Thread.Sleep(delayBetween);

                RandomClipping rc = new RandomClipping(fullScreenBitmap, DisplayFont);
                Thread.Sleep(delayBetween);

                MatrixRain mr = new MatrixRain(fullScreenBitmap);
                Thread.Sleep(Timeout.Infinite);
            }
        }


        static SimpleTouchInput myApplicationSimpleTouch;
        private static void SimpleTouch()
        {
            myApplicationSimpleTouch = new SimpleTouchInput();

            // Touch notifications are not turned on by default.  You need to 
            // explicitly inform the Touch engine that you want touch events 
            // to be pumped to your direction, and you want to work with the 
            // rest of the architecture.

            Touch.Initialize(myApplicationSimpleTouch);

            Window mainWindow = myApplicationSimpleTouch.CreateWindow();

            // Start the application.
            myApplicationSimpleTouch.Run(mainWindow);
        }

        //    static MyPuzzle myApplicationPuzzle;
        //    private static void PuzzleProgram()
        //    {
        //        myApplicationPuzzle = new MyPuzzle();

        //        // Touch notifications are not turned on by default.  You need to 
        //        // explicitly inform the Touch engine that you want touch events 
        //        // to be pumped to your direction, and you want to work with the 
        //        // rest of the architecture.

        //        Touch.Initialize(myApplicationPuzzle);

        //        Window mainWindow = myApplicationPuzzle.CreateWindow();

        //        // Start the application.
        //        myApplicationPuzzle.Run(mainWindow);
        //    }
    }
}
