﻿
using nanoFramework.Presentation;
using nanoFramework.Presentation.Controls;
using nanoFramework.Presentation.Media;
using nanoFramework.UI;
using nanoFramework.UI.Input;
using nanoFramework.UI.Threading;
using Primitives;
using System;


namespace PuzzleSample
{
    /// <summary>
    /// Puzzle demonstrating a touch panel.
    /// </summary>
    public class MyPuzzle : nanoFramework.UI.Application
    {
        /// <summary>
        /// Defines a Button.
        /// </summary>
        protected class Button : UIElement
        {
            /// <summary>
            /// Constructs a Button with the specified caption and font, and 
            /// default size.
            /// </summary>
            /// <param name="caption"></param>
            /// <param name="font"></param>
            public Button(string caption, Font font)
            {
                _caption = caption;
                _font = font;
                HorizontalAlignment = HorizontalAlignment.Left;
                VerticalAlignment = VerticalAlignment.Bottom;

                int textWidth;
                int textHeight;
                _font.ComputeExtent(_caption, out textWidth, out textHeight);

                _width = textWidth + _textMarginX * 2;
                _height = textHeight + _textMarginY * 2;
            }

            /// <summary>
            /// Constructs a Button with the specified caption, font, and size.
            /// </summary>
            /// <param name="caption"></param>
            /// <param name="font"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            public Button(string caption, Font font, int width, int height)
            {
                _width = width;
                _height = height;
                _caption = caption;
                _font = font;
            }

            /// <summary>
            /// Overrides OnRender to do our own drawing.
            /// </summary>
            /// <param name="dc"></param>
            public override void OnRender(DrawingContext dc)
            {
                int x;
                int y;

                SolidColorBrush brush;
                Pen pen;
                Color color;
                Pen shade1;
                Pen shade2;

                // Check the pressed state and draw accordingly.
                if (_pressed)
                {
                    brush = _pressedBackgroundBrush;
                    pen = _pressedBorderPen;
                    color = _pressedForeColor;
                    shade1 = _darkShade;
                    shade2 = _lightShade;
                }
                else
                {
                    brush = _normalBackgroundBrush;
                    pen = _borderPen;
                    color = _foreColor;
                    shade1 = _lightShade;
                    shade2 = _darkShade;
                }

                GetLayoutOffset(out x, out y);

                // Draw the base rectangle of the button.
                dc.DrawRectangle(brush, pen, 1, 1, _width - 1, _height - 1);

                // Draw the caption.
                string caption = _caption;
                dc.DrawText(ref caption, _font, color, 0, _textMarginY, _width, _height, _alignment, _trimming);

                // Shade the outline of the rectangle for classic button look.
                dc.DrawLine(shade1, 1, 1, _width - 1, 1);
                dc.DrawLine(shade1, 1, 1, 1, _height - 1);
                dc.DrawLine(shade2, _width - 1, 1, _width - 1, _height - 1);
                dc.DrawLine(shade2, 1, _height - 1, _width - 1, _height - 1);
            }

            public event EventHandler Click;

            /// <summary>
            /// Handles the touch down event.
            /// </summary>
            /// <param name="e"></param>
            protected override void OnTouchDown(TouchEventArgs e)
            {
                // Flag for drawing state.
                _pressed = true;

                // Capture the touch.
                TouchCapture.Capture(this);

                // Trigger a redraw.
                Invalidate();
            }

            /// <summary>
            /// Handles the touch up event.
            /// </summary>
            /// <param name="e"></param>
            protected override void OnTouchUp(TouchEventArgs e)
            {
                // Flag for the drawing state.
                _pressed = false;

                // Capture the touch.
                TouchCapture.Capture(this, CaptureMode.None);

                // Trigger a redraw.
                Invalidate();

                // Fire a click event.
                EventArgs args = new EventArgs();
                OnClick(args);
            }

            /// <summary>
            /// Handles the touch move.
            /// </summary>
            /// <param name="e"></param>
            protected override void OnTouchMove(TouchEventArgs e)
            {
            }

            /// <summary>
            /// Handles a click.
            /// </summary>
            /// <param name="e"></param>
            protected virtual void OnClick(EventArgs e)
            {
                if (Click != null)
                {
                    Click(this, e);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="arrangeWidth"></param>
            /// <param name="arrangeHeight"></param>
            protected override void ArrangeOverride(int arrangeWidth, int arrangeHeight)
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="availableWidth"></param>
            /// <param name="availableHeight"></param>
            /// <param name="desiredWidth"></param>
            /// <param name="desiredHeight"></param>
            protected override void MeasureOverride(int availableWidth, int availableHeight, out int desiredWidth, out int desiredHeight)
            {
                desiredWidth = (availableWidth > _width) ? _width : availableWidth;
                desiredHeight = (availableHeight > _height) ? _height : availableHeight;
            }

            private SolidColorBrush _normalBackgroundBrush = new SolidColorBrush(ColorUtility.ColorFromRGB(192, 192, 192));
            private SolidColorBrush _pressedBackgroundBrush = new SolidColorBrush(ColorUtility.ColorFromRGB(128, 128, 128));
            private Pen _borderPen = new Pen(ColorUtility.ColorFromRGB(128, 128, 128));
            private Pen _pressedBorderPen = new Pen(ColorUtility.ColorFromRGB(128, 128, 128));
            private Pen _lightShade = new Pen(ColorUtility.ColorFromRGB(216, 216, 216));
            private Pen _darkShade = new Pen(ColorUtility.ColorFromRGB(64, 64, 64));

            int _width;
            int _height;
            string _caption = "";
            Font _font = null;
            Color _foreColor = ColorUtility.ColorFromRGB(0, 0, 0);
            Color _pressedForeColor = ColorUtility.ColorFromRGB(255, 255, 255);
            private TextTrimming _trimming = TextTrimming.WordEllipsis;
            private TextAlignment _alignment = TextAlignment.Center;
            protected int _textMarginX = 16;
            protected int _textMarginY = 8;
            protected bool _pressed = false;
        }

        /// <summary>
        /// A canvas class based on the .NET Micro Framework InkCanvas class.
        /// </summary>
        protected class PuzzleBoard : InkCanvas
        {
            Pen pen = new Pen(ColorUtility.ColorFromRGB(0, 0, 0), 2);
            Bitmap[] _blockBitmap = new Bitmap[9];
            private DispatcherTimer _animationTimer;

            int blockDimension = 0;
            int[] _blocks = new int[9];

            /// <summary>
            /// Constructs a PuzzleBoard with the specified settings.
            /// </summary>
            /// <param name="width"></param>
            /// <param name="height"></param>
            public PuzzleBoard(int width, int height) : base(0, 0, width, height)
            {
                blockDimension = (width > height ? height : width) / 3;

                // Use native collection and only look for gestures.
                TouchCollectorConfiguration.CollectionMethod =
                    CollectionMethod.Native;
                TouchCollectorConfiguration.CollectionMode =
                    CollectionMode.InkAndGesture;

                // Load all of the puzzle images

                _blockBitmap[0] = new Bitmap(Resource.GetBitmap(Resource.BitmapResources.b1),Bitmap.BitmapImageType.Jpeg);
                _blockBitmap[1] = new Bitmap(Resource.GetBitmap(Resource.BitmapResources.b2),Bitmap.BitmapImageType.Jpeg);
                _blockBitmap[2] = new Bitmap(Resource.GetBitmap(Resource.BitmapResources.b3),Bitmap.BitmapImageType.Jpeg);
                _blockBitmap[3] = new Bitmap(Resource.GetBitmap(Resource.BitmapResources.b4),Bitmap.BitmapImageType.Jpeg);
                _blockBitmap[4] = new Bitmap(Resource.GetBitmap(Resource.BitmapResources.b5),Bitmap.BitmapImageType.Jpeg);
                _blockBitmap[5] = new Bitmap(Resource.GetBitmap(Resource.BitmapResources.b6),Bitmap.BitmapImageType.Jpeg);
                _blockBitmap[6] = new Bitmap(Resource.GetBitmap(Resource.BitmapResources.b7),Bitmap.BitmapImageType.Jpeg);
                _blockBitmap[7] = new Bitmap(Resource.GetBitmap(Resource.BitmapResources.b8),Bitmap.BitmapImageType.Jpeg);
                _blockBitmap[8] = new Bitmap(Resource.GetBitmap(Resource.BitmapResources.b9), Bitmap.BitmapImageType.Jpeg);

                // Set up the animation timer.
                _animationTimer = new DispatcherTimer(this.Dispatcher);
                _animationTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
                _animationTimer.Tick += new EventHandler(AnimationTimer_Tick);

                Reset();
            }

            /// <summary>
            /// Handles the animation timer tick.
            /// </summary>
            int _animationStep = 0;
            void AnimationTimer_Tick(object sender, EventArgs e)
            {
                if (_animationStep >= 10)
                {
                    _animationTimer.Stop();
                }
                else
                {
                    _animationStep++;
                    Invalidate();
                }
            }

            // Offset amounts for animation.  These values can be used to change 
            // the X or Y location.  See below for usage.
            int[] anim1 = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] anim2 = new int[] { -5, 0, 5, 0, -5, 0, 5, 0, -5, 0 };
            int[] anim3 = new int[] { 80, 74, 68, 58, 48, 36, 23, 12, 5, 0 };
            int[] anim4 = new int[] { -80, -74, -68, -58, -48, -36, -23, -12, -5, 0 };
            int[] anim5 = new int[] { 2, 6, 11, 15, 16, 15, 11, 6, 2, 0 };
            int[] anim6 = new int[] { -2, -6, -11, -15, -16, -15, -11, -6, -2, 0 };

            int[] activeAnimX = null;
            int[] activeAnimY = null;
            int _animationTargetBlock = -1;

            /// <summary>
            /// Handles gesture animation.
            /// </summary>
            /// <param name="e"></param>
            protected override void OnTouchGestureChanged(TouchGestureEventArgs e)
            {
                // Find the affected block.
                int x = e.X;
                int y = e.Y;

                int c = x / blockDimension;
                int r = y / blockDimension;
                int b = c + r * 3;

                int nc = c;
                int nr = r;

                int[] altAnimX = null;
                int[] altAnimY = null;

                _animationTargetBlock = b;

                // The gesture event tells us what direction the user gestured.
                // Based on the gesture, use the animation offset arrays to move 
                // the block to the appropriate location.
                switch (e.Gesture)
                {
                    case TouchGesture.Left:
                        {
                            nc = nc - 1;

                            activeAnimX = anim3;
                            activeAnimY = anim1;

                            altAnimX = anim6;
                            altAnimY = anim1;

                            break;
                        }
                    case TouchGesture.Right:
                        {
                            nc = nc + 1;

                            activeAnimX = anim4;
                            activeAnimY = anim1;

                            altAnimX = anim5;
                            altAnimY = anim1;

                            break;
                        }
                    case TouchGesture.Up:
                        {
                            nr = nr - 1;

                            activeAnimX = anim1;
                            activeAnimY = anim3;

                            altAnimX = anim1;
                            altAnimY = anim6;

                            break;
                        }
                    case TouchGesture.Down:
                        {
                            nr = nr + 1;

                            activeAnimX = anim1;
                            activeAnimY = anim4;

                            altAnimX = anim1;
                            altAnimY = anim5;

                            break;
                        }
                    case TouchGesture.UpLeft:
                        {
                            nc = -1;
                            nr = -1;

                            activeAnimX = anim6;
                            activeAnimY = anim6;

                            break;
                        }
                    case TouchGesture.UpRight:
                        {
                            nc = -1;
                            nr = -1;

                            activeAnimX = anim5;
                            activeAnimY = anim6;

                            break;
                        }
                    case TouchGesture.DownLeft:
                        {
                            nc = -1;
                            nr = -1;

                            activeAnimX = anim6;
                            activeAnimY = anim5;

                            break;
                        }
                    case TouchGesture.DownRight:
                        {
                            nc = -1;
                            nr = -1;

                            activeAnimX = anim5;
                            activeAnimY = anim5;

                            break;
                        }

                    default:
                        {
                            nc = -1;
                            nr = -1;

                            activeAnimX = anim2;
                            activeAnimY = anim1;

                            break;
                        }
                }

                // Verify that the target block location is empty.
                if ((nc >= 0) && (nc < 3) && (nr >= 0) && (nr < 3))
                {
                    int nb = nc + nr * 3;
                    if (_blocks[nb] == 8)
                    {
                        /// Target location is empty.
                        _blocks[nb] = _blocks[b];
                        _blocks[b] = 8;

                        _animationTargetBlock = nb;
                    }
                    else
                    {
                        activeAnimX = altAnimX;
                        activeAnimY = altAnimY;
                    }
                }
                else if (altAnimX != null)
                {
                    activeAnimX = altAnimX;
                    activeAnimY = altAnimY;
                }

                if (activeAnimX != null && activeAnimY != null)
                {
                    _animationStep = 0;
                    _animationTimer.Start();
                }

            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="i"></param>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <param name="dc"></param>
            private void DrawBlock(int i, int x, int y, DrawingContext dc)
            {
                dc.DrawImage(_blockBitmap[i], x, y);
            }

            /// <summary>
            /// Handles the render event.
            /// </summary>
            /// <param name="dc"></param>
            public override void OnRender(DrawingContext dc)
            {
                // Loop through all the blocks except the animated block
                // and draw the associated bitmap.
                int i = 0;
                for (i = 0; i < 9; i++)
                {
                    int x = (i % 3) * blockDimension;
                    int y = (i / 3) * blockDimension;

                    if (_animationTargetBlock != i)
                    {
                        DrawBlock(_blocks[i], x, y, dc);
                    }
                }

                // Draw the animated block if there is one.
                if (_animationTargetBlock >= 0)
                {
                    lock (_blockBitmap)
                    {
                        int x = (_animationTargetBlock % 3) * blockDimension;
                        int y = (_animationTargetBlock / 3) * blockDimension;

                        dc.DrawImage(_blockBitmap[8], x, y);

                        if (_animationStep < 9 && activeAnimX != null && activeAnimY != null)
                        {
                            x += activeAnimX[_animationStep];
                            y += activeAnimY[_animationStep];
                        }

                        DrawBlock(_blocks[_animationTargetBlock], x, y, dc);
                    }
                }

                // Draw a rectangle box around the puzzle.
                dc.DrawLine(pen, 0, 0, 0, 239);
                dc.DrawLine(pen, 0, 0, 240, 0);
                dc.DrawLine(pen, 0, 239, 240, 239);
                dc.DrawLine(pen, 240, 0, 240, 239);
            }

            /// <summary>
            /// Put all of the blocks in the right place automatically for the 
            /// user.
            /// </summary>
            public void Solve()
            {
                int i = 0;
                for (i = 0; i < 9; i++)
                {
                    _blocks[i] = i;
                }
                Invalidate();
            }

            /// <summary>
            /// Put all of the blocks in the starting locations.  This is always 
            /// the same, to make sure it is solvable.
            /// <para>
            /// A challenge for the reader would be to come up with an algorithm 
            /// to randomize the blocks in such a way that the puzzle is still 
            /// solvable.
            /// </para>
            /// </summary>
            public void Reset()
            {
                _blocks[0] = 3;
                _blocks[1] = 8;
                _blocks[2] = 0;
                _blocks[3] = 6;
                _blocks[4] = 2;
                _blocks[5] = 5;
                _blocks[6] = 1;
                _blocks[7] = 4;
                _blocks[8] = 7;
                Invalidate();
            }
        }

        /// <summary>
        /// The main window class, based on the standard Window.
        /// </summary>
        protected class MyWindow : Window
        {
            Panel panel = new Panel();
            PuzzleBoard puzzleBoard = null;
            Button button = null;
            Button button2 = null;
            Text text = null;

            /// <summary>
            /// The default constructor.
            /// </summary>
            public MyWindow()
            {
                int screenWidth = DisplayControl.ScreenWidth;
                int screenHeight = DisplayControl.ScreenHeight;
                int bitsPerPixel = DisplayControl.BitsPerPixel;

                // Create the puzzle board.  Default to square, because all of 
                // the images fit in a square.
                puzzleBoard = new PuzzleBoard(240, 240);

                puzzleBoard.TouchGestureChanged += new TouchGestureEventHandler(puzzleBoard_Gesture);

                // Create the Reset button.
                button = new Button("Reset", Resource.GetFont(Resource.FontResources.small));
                button.HorizontalAlignment = HorizontalAlignment.Right;
                button.VerticalAlignment = VerticalAlignment.Top;
                button.Click += new EventHandler(button_Click);

                // Create the Solve button.
                button2 = new Button("Solve", Resource.GetFont(Resource.FontResources.small));
                button2.HorizontalAlignment = HorizontalAlignment.Right;
                button2.VerticalAlignment = VerticalAlignment.Center;
                button2.Click += new EventHandler(button2_Click);

                if (screenWidth < screenHeight)
                {
                    button.HorizontalAlignment = HorizontalAlignment.Left;
                    button.VerticalAlignment = VerticalAlignment.Bottom;

                    button2.HorizontalAlignment = HorizontalAlignment.Center;
                    button2.VerticalAlignment = VerticalAlignment.Bottom;
                }

                // Create a text element to display the action the user just 
                // performed.
                text = new Text();
                text.Font = Resource.GetFont(Resource.FontResources.small);
                text.TextContent = " ";
                text.HorizontalAlignment = nanoFramework.Presentation.HorizontalAlignment.Right;
                text.VerticalAlignment = nanoFramework.Presentation.VerticalAlignment.Bottom;

                // Add a panel to hold the other controls.
                this.Child = panel;

                // Add the puzzle, buttons and text controls to the panel.
                panel.Children.Add(puzzleBoard);
                panel.Children.Add(button);
                panel.Children.Add(button2);
                panel.Children.Add(text);

                // Set the drawing attributes to a default set.
                DrawingAttributes da = new DrawingAttributes();
                puzzleBoard.DefaultDrawingAttributes = da;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            void puzzleBoard_Gesture(object sender, TouchGestureEventArgs e)
            {
                string gesture = "Unknown";

                switch (e.Gesture)
                {
                    case TouchGesture.Right:
                        gesture = "Right";
                        break;
                    case TouchGesture.UpRight:
                        gesture = "UpRight";
                        break;
                    case TouchGesture.Up:
                        gesture = "Up";
                        break;
                    case TouchGesture.UpLeft:
                        gesture = "UpLeft";
                        break;
                    case TouchGesture.Left:
                        gesture = "Left";
                        break;
                    case TouchGesture.DownLeft:
                        gesture = "DownLeft";
                        break;
                    case TouchGesture.Down:
                        gesture = "Down";
                        break;
                    case TouchGesture.DownRight:
                        gesture = "DownRight";
                        break;
                    case TouchGesture.Tap:
                        gesture = "Tap";
                        break;
                    case TouchGesture.DoubleTap:
                        gesture = "DoubleTap";
                        break;

                    case TouchGesture.Zoom:
                        gesture = "Zoom";
                        break;
                    case TouchGesture.Pan:
                        gesture = "Pan";
                        break;
                    case TouchGesture.Rotate:
                        gesture = "Rotate: " + e.Angle.ToString();
                        break;
                    case TouchGesture.TwoFingerTap:
                        gesture = "TwoFingerTap";
                        break;
                    case TouchGesture.Rollover:
                        gesture = "Rollover";
                        break;
                }

                text.TextContent = gesture;
            }

            /// <summary>
            /// Handles the Reset button click.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            void button_Click(object sender, EventArgs e)
            {
                puzzleBoard.Reset();
            }

            /// <summary>
            /// Handles the Solve button click.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            void button2_Click(object sender, EventArgs e)
            {
                puzzleBoard.Solve();
            }
        }


        /// <summary>
        /// The executable entry point.
        /// </summary>

        private MyWindow mainWindow;

        /// <summary>
        /// Create a window with button focus.
        /// </summary>
        /// <returns></returns>
        public Window CreateWindow()
        {
            /// Create a window and set its size to the size of the display.
            mainWindow = new MyWindow();
            mainWindow.Height = DisplayControl.ScreenHeight;
            mainWindow.Width = DisplayControl.ScreenWidth;

            // Set the window visibility to visible.
            mainWindow.Visibility = Visibility.Visible;

            // Attach the button focus to the window.
            Buttons.Focus(mainWindow);

            return mainWindow;
        }
    }
}
