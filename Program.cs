namespace kowder
{
    using System;
    using SkiaSharp;
    using System.Threading.Tasks;

    class Program
    {
        public static SKPaint surface = new SKPaint { Color = new SKColor(31, 31, 31, 255) };
        public static SKPaint bg = new SKPaint { Color = SKColor.Parse("#121212") };
        public static float CaretAlpha = 0;
        private static SideBar sideBar = new SideBar();
        private static int topAnchor = 35;
        static void Main(string[] args)
        {
            Task.Run(() => Packages.Load());

            Task.Run(() => Window.SetRenderMethod(delegate ()
            {
                var canvas = Window.GetCanvas();

                canvas.DrawRect(0, 0, Window.Size.Width, topAnchor, surface);

                sideBar.Draw(topAnchor);

                var TextPaint = new SKPaint { Color = new SKColor(255, 255, 255, 255), TextSize = 25 };
                canvas.DrawText(
                    Window.typedContent,
                    248, 50,
                    TextPaint
                );

                var Alpha = MathF.Abs((MathF.Sin(CaretAlpha) * 255));
                var CaretPaint = new SKPaint
                {
                    Color = new SKColor(255, 255, 255, (byte)Alpha)
                };

                var width = TextPaint.MeasureText(Window.typedContent);
                canvas.DrawRect(
                    248 + width, 25,
                    2, 25,
                    CaretPaint
                );

                CaretAlpha += 0.01f;
            })
            );

            Window.Init();
        }

        private static void SideBar()
        {
            sideBar.Draw(topAnchor);
        }
    }
}