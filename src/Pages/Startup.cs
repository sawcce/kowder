namespace kowder
{
    using SkiaSharp;
    class Startup 
    {
        public static SKCanvas canvas;
        public static void Init() 
        {
            Window.SetRenderMethod(delegate () {
                canvas.DrawRect(0,0, Window.Size.Width, Window.Size.Height, Themes.surface);

                canvas.DrawText("Welcome Kowder!", new SKPoint(100 ,100), Themes.TextPaint);
            });
        }
    }
}