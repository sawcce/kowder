namespace kowder
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using GLFW;
    using SkiaSharp;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    class Window
    {
        private static long lastTime = DateTimeOffset.Now.ToUnixTimeSeconds();
        private static NativeWindow window;
        private static SKCanvas canvas;
        private static GRContext context;
        private static SKSurface skiaSurface;
        private static Keys? lastKeyPressed;
        public static Point lastMousePosition = new Point(0, 0);
        private static float i = 0;
        private static Action render = delegate () { };
        private static Size windowSize = new Size(800, 600);
        //private static Dictionary<MouseButton, bool> lastMouseButtons = new Dictionary<MouseButton, bool>();
        private static Dictionary<MouseButton, InputState> mouseButtons = new Dictionary<MouseButton, InputState>();
        public static Dictionary<int, bool> keys = new Dictionary<int, bool>();
        private static Point dragOffset = new Point(0, 0);
        private static bool dragging = false;
        public static Size Size { get { return windowSize; } }
        public static string typedContent = "";


        // Events
        public static event EventHandler<MouseButton> MouseButtonPressed;
        // More specific events to avoid if statements everywhere
        public delegate void NoArgs();
        public static event NoArgs LeftMouseButtonPressed;

        #region  setup
        public static void Init()
        {
            Glfw.WindowHint(GLFW.Hint.Decorated, true);
            Glfw.WindowHint(GLFW.Hint.Resizable, true);

            using (Window.window = new NativeWindow(800, 600, ""))
            {
                Window.SubscribeToWindowEvents();
                IntPtr hWnd = window.Handle;

                // Limits minimum window size because couldn't find
                // GLFW_DONT_CARE 
                // https://www.glfw.org/docs/3.3/window_guide.html#window_sizelimits
                window.SetSizeLimits(450, 350, 100000, 100000);

                using (Window.context = Window.GenerateSkiaContext(Window.window))
                {
                    using (Window.skiaSurface = Window.GenerateSkiaSurface(context, Window.window.ClientSize))
                    {
                        Window.canvas = skiaSurface.Canvas;
                        windowSize = new Size(800, 600);

                        Startup.canvas = canvas;
                        Startup.SetCanvas();
                        KowderEditor.canvas = canvas;
                        while (!Window.window.IsClosing)
                        {
                            Glfw.PollEvents();
                            // Get difference of time (ms) between last render call
                            // and current time
                            var diff = lastTime - DateTimeOffset.Now.ToUnixTimeSeconds();

                            // If more than 1/60 of a sec has elapsed since last render call
                            // re-render
                            // Effect: Sets a cap of 60fps
                            if (Math.Abs(diff) >= 1 / 60)
                            {
                                Window.Render();
                                lastTime = DateTimeOffset.Now.ToUnixTimeSeconds();
                            }
                        }
                    }
                }
            }
        }

        /// <summary> Add the event handlers to their respective events </summary>
        private static void SubscribeToWindowEvents()
        {
            Window.window.SizeChanged += Window.OnWindowsSizeChanged;
            Window.window.Refreshed += Window.OnWindowRefreshed;
            Window.window.KeyPress += Window.OnWindowKeyPress;
            Window.window.KeyRelease += Window.OnWindowKeyReleased;
            Window.window.MouseMoved += Window.OnWindowMouseMoved;
            Window.window.MouseButton += Window.OnMouseAction;
        }

        /// <summary>Generates the skia context using a window element</summary>
        private static GRContext GenerateSkiaContext(NativeWindow nativeWindow)
        {
            var nativeContext = Window.GetNativeContext(nativeWindow);
            var glInterface = GRGlInterface.AssembleGlInterface(nativeContext, (contextHandle, name) => Glfw.GetProcAddress(name));
            return GRContext.CreateGl(glInterface);
        }

        private static object GetNativeContext(NativeWindow nativeWindow)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Native.GetWglContext(nativeWindow);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // XServer
                return Native.GetGLXContext(nativeWindow);
                // Wayland
                //return Native.GetEglContext(nativeWindow);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Native.GetNSGLContext(nativeWindow);
            }

            throw new PlatformNotSupportedException();
        }

        /// <summary>Generates the skia surface used for the canvas</summary>

        private static SKSurface GenerateSkiaSurface(GRContext skiaContext, Size surfaceSize)
        {
            var frameBufferInfo = new GRGlFramebufferInfo((uint)new UIntPtr(0), GRPixelConfig.Rgba8888.ToGlSizedFormat());
            var backendRenderTarget = new GRBackendRenderTarget(surfaceSize.Width,
                                                                surfaceSize.Height,
                                                                0,
                                                                8,
                                                                frameBufferInfo);
            return SKSurface.Create(skiaContext, backendRenderTarget, GRSurfaceOrigin.BottomLeft, SKImageInfo.PlatformColorType);
        }

        #endregion setup
        /// <summary>
        /// Function called every rendered frame
        /// </summary>
        private static void Render()
        {

            Window.canvas.Clear(SKColor.Parse("#121212"));


            if (render != null) render.Invoke();

            Window.canvas.Flush();
            context.Flush();
            Window.window.SwapBuffers();
        }

        public static void SetRenderMethod(Action callback)
        {
            render = callback;
        }

        public static Size GetSize()
        {
            return Window.window.ClientSize;
        }

        public static SKCanvas GetCanvas()
        {
            return Window.skiaSurface.Canvas;
        }

        public static Keys? GetLastKeyPressed()
        {
            return Window.lastKeyPressed;
        }

        public static Point? GetMousePosition()
        {
            if (lastMousePosition == null)
            {
                return new Point(0, 0);
            }
            return Window.lastMousePosition;
        }

        #region Window Events Handlers

        private static void OnWindowsSizeChanged(object sender, SizeChangeEventArgs e)
        {
            var size = e.Size;

            window.ClientSize = size;
            windowSize = size;
            skiaSurface = GenerateSkiaSurface(context, size);
            canvas = skiaSurface.Canvas;

            // Must reasign the canvas as else it won't be changed
            
            Startup.canvas = canvas;
            Startup.SetCanvas();

            KowderEditor.canvas = canvas;
            KowderEditor.SizeChanged();

            Window.Render();
        }

        /// <summary>
        /// Event called whenever a key is released
        /// </summary>
        private static void OnWindowKeyReleased(object sender, KeyEventArgs e)
        {
            // Checks if the key has actually been released
            // because the event gets fired when a key is being held (InputState.Repeat)
            if (e.State == InputState.Release)
            {
                Window.keys[e.ScanCode] = false;
            }
        }

        /// <summary>
        /// Event called whenever a key is pressed
        /// </summary>
        private static void OnWindowKeyPress(object sender, KeyEventArgs e)
        {
            Window.lastKeyPressed = e.Key;
            Window.keys[e.ScanCode] = true;

            var str = KeyboardLayouts.GetKey(e.ScanCode);

            switch (str)
            {
                case "Backspace":
                    if (typedContent.Length > 0)
                    {
                        typedContent = typedContent.Remove(typedContent.Length - 1);
                    }
                    break;
                case "undefined":
                    break;
            }

            //Console.WriteLine("{0} {1}  {2}", e.Key.ToString(), str, e.ScanCode.ToString());
            if (str?.Length == 1)
            {
                typedContent += str;
            }
            // if (e.Key == Keys.Enter || e.Key == Keys.NumpadEnter)
            // {
            //     Window.window.Close();
            // }
        }

        private static void OnWindowMouseMoved(object sender, MouseMoveEventArgs e)
        {
            Window.lastMousePosition = e.Position;
        }

        private static void OnWindowRefreshed(object sender, EventArgs e)
        {
            Window.Render();
        }

        private static void OnMouseAction(object sender, MouseButtonEventArgs e)
        {
            if(e.Action == InputState.Press)
            {
                MouseButtonPressed?.Invoke(null, e.Button);

                if(e.Button == MouseButton.Left) 
                {
                    LeftMouseButtonPressed?.Invoke();
                }
            }
            
            mouseButtons[e.Button] = e.Action;
        }

        #endregion

        #region Helpers
        /// <summary>
        /// Safe wrapper to get the current state of a key
        /// even if it doesn't exist in the dictionary
        /// </summary>
        public static bool GetKeyPressed(int scanCode)
        {
            var pressed = false;
            keys.TryGetValue(scanCode, out pressed);

            return pressed;
        }

        /// <summary>
        /// Safe wrapper to get the current state of a key
        /// even if it doesn't exist in the dictionary
        /// if one key isn't pressed it'll check for the other key
        /// and return the result of the or operator on the two
        /// bools
        /// </summary>
        public static bool GetKeyPressed(int scanCode, int scanCode2)
        {
            var pressed = false;
            keys.TryGetValue(scanCode, out pressed);

            var pressed2 = false;
            keys.TryGetValue(scanCode2, out pressed2);

            return pressed || pressed2;
        }

        /// <summary>
        /// Checks if the mouse's position is between x and xx on the x axis,
        /// and y and yy on the y axis.
        /// With x < xx and y < yy.
        /// </summary>
        public static bool IsCursorInBounds(int x, int xx, int y, int yy)
        {
            var mousePos = Window.GetMousePosition().Value;

            var inBoundsX = (mousePos.X >= x && mousePos.X <= xx);
            if (!inBoundsX) return false;

            var inBoundsY = mousePos.Y >= y && mousePos.Y <= yy;
            if (!inBoundsY) return false;

            return true;
        }

        /// <summary>
        /// <para>
        /// Checks if the mouse's position is in the bounds of a vertical list and if so,
        /// returns the index number that the mouse is hovering.
        /// </para>
        /// With <paramref name="x"/> and <paramref name="xx"/> being the bounds of the list on the x axis <para/>
        /// <paramref name="top"/> being the coordinates of the list's top <para/>
        /// <paramref name="itemHeight"/> being the height of the items <para/>
        /// and <paramref name="itemCount"/> being the number of items in the list <para/> <para/>
        /// NOTE: This only works for lists where the item height is fixed and known when the function
        /// is called.
        /// </summary>
        public static (bool, int) SmartCursorBoundsVertical(int x, int xx, int top, int itemHeight, int itemCount)
        {
            var inBoundsX = lastMousePosition.X >= x && lastMousePosition.X <= xx;
            if(!inBoundsX) return (false, -1);

            var py = lastMousePosition.Y - top;
            if(py < 0) return (false, -1);

            var yIndex = py / itemHeight;

            var inBoundsY = yIndex < itemCount;
            if(!inBoundsY) return (false, -1);

            return (true, yIndex);
        }
        #endregion
    }
}