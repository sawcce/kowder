namespace kowder
{
    using System;
    using SkiaSharp;

    class Program
    {
        public static SKPaint surface = new SKPaint { Color = new SKColor(31, 31, 31, 255) };
        public static SKPaint bg = new SKPaint { Color = SKColor.Parse("#121212") };
        static void Main(string[] args)
        {

            var sideBar = new SideBar();

            Window.SetRenderMethod(delegate ()
            {
                var canvas = Window.GetCanvas();

                var topAnchor = 35;
                canvas.DrawRect(0, 0, Window.Size.Width, topAnchor, surface);
                sideBar.Draw(topAnchor);
            });

            Window.Init();
        }
    }
}