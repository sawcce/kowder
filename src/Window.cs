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
        private static Size windowSize;
        //private static Dictionary<MouseButton, bool> lastMouseButtons = new Dictionary<MouseButton, bool>();
        private static Dictionary<MouseButton, InputState> mouseButtons = new Dictionary<MouseButton, InputState>();
        public static Dictionary<int, bool> keys = new Dictionary<int, bool>();
        private static Point dragOffset = new Point(0, 0);
        private static bool dragging = false;
        public static Size Size { get { return windowSize; } }
        public static string typedContent = "";

        #region  setup
        public static void Init()
        {
            Glfw.WindowHint(GLFW.Hint.Decorated, true);
            Glfw.WindowHint(GLFW.Hint.Resizable, true);

            using (Window.window = new NativeWindow(800, 600, ""))
            {
                Window.SubscribeToWindowEvents();
                IntPtr hWnd = window.Handle;

                using (Window.context = Window.GenerateSkiaContext(Window.window))
                {
                    using (Window.skiaSurface = Window.GenerateSkiaSurface(context, Window.window.ClientSize))
                    {
                        Window.canvas = skiaSurface.Canvas;
                        windowSize = window.ClientSize;
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
            Window.window.MouseButton += Window.OnMousePressed;
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
            windowSize = window.ClientSize;
            skiaSurface = GenerateSkiaSurface(context, e.Size);
            canvas = skiaSurface.Canvas;
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

        private static void OnMousePressed(object sender, MouseButtonEventArgs e)
        {
            mouseButtons[e.Button] = e.Action;
        }

        #endregion

        #region Helpers
        public static bool IsCursorInBounds(int x, int xx, int y, int yy)
        {
            var mousePos = Window.GetMousePosition().Value;

            var inBoundsX = (mousePos.X >= x && mousePos.X <= xx);
            if (!inBoundsX) return false;

            var inBoundsY = mousePos.Y >= y && mousePos.Y <= yy;
            if (!inBoundsY) return false;

            return true;
        }
        #endregion
    }
}