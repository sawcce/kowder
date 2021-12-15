namespace kowder
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using GLFW;
    using SkiaSharp;
    using System.Collections.Generic;

    class Window
    {
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
        private static NativeWindow window;
        private static SKCanvas canvas;
        private static GRContext context;
        private static SKSurface skiaSurface;
        private static Keys? lastKeyPressed;
        private static Point? lastMousePosition;
        private static float i = 0;
        private static Action render = delegate () { };
        private static Size windowSize;
        //private static Dictionary<MouseButton, bool> lastMouseButtons = new Dictionary<MouseButton, bool>();
        private static Dictionary<MouseButton, InputState> mouseButtons = new Dictionary<MouseButton, InputState>();
        private static Point dragOffset = new Point(0, 0);
        private static bool dragging = false;
        public static Size Size { get { return windowSize; } }

        //----------------------------------
        //NOTE: On Windows you must copy SharedLib manually (https://github.com/ForeverZer0/glfw-net#microsoft-windows)
        //----------------------------------

        #region  setup
        public static void Init()
        {
            Glfw.WindowHint(GLFW.Hint.Decorated, true);
            Glfw.WindowHint(GLFW.Hint.Resizable, true);

            using (Window.window = new NativeWindow(800, 600, ""))
            {
                Window.SubscribeToWindowEvents();
                IntPtr hWnd = window.Handle;
                SetWindowLong(hWnd, -20, 1);

                using (Window.context = Window.GenerateSkiaContext(Window.window))
                {
                    using (Window.skiaSurface = Window.GenerateSkiaSurface(context, Window.window.ClientSize))
                    {
                        Window.canvas = skiaSurface.Canvas;
                        windowSize = window.ClientSize;
                        while (!Window.window.IsClosing)
                        {
                            Window.Render();
                            Glfw.PollEvents();

                        }
                    }
                }
            }
        }

        private static void SubscribeToWindowEvents()
        {
            Window.window.SizeChanged += Window.OnWindowsSizeChanged;
            Window.window.Refreshed += Window.OnWindowRefreshed;
            Window.window.KeyPress += Window.OnWindowKeyPress;
            Window.window.MouseMoved += Window.OnWindowMouseMoved;
            Window.window.MouseButton += Window.OnMousePressed;
        }

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

        private static void OnWindowKeyPress(object sender, KeyEventArgs e)
        {
            Window.lastKeyPressed = e.Key;
            if (e.Key == Keys.Enter || e.Key == Keys.NumpadEnter)
            {
                Window.window.Close();
            }
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