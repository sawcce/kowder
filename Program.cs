namespace kowder
{
    using System;
    using SkiaSharp;
    using System.Threading.Tasks;

    class Program
    {
        public static SKPaint surface = new SKPaint { Color = new SKColor(31, 31, 31, 255) };
        public static SKPaint bg = new SKPaint { Color = SKColor.Parse("#121212") };
        private static SideBar sideBar = new SideBar();
        private static Carret carret = new Carret(0, 0);
        private static int topAnchor = 35;
        static void Main(string[] args)
        {
            Task.Run(() => Packages.Load());

            var TextPaint = new SKPaint { Color = new SKColor(255, 255, 255, 255), TextSize = 25 };
            TextPaint.Typeface = SKTypeface.FromFile("res/um.ttf");

            Task.Run(() => Window.SetRenderMethod(delegate ()
            {
                var canvas = Window.GetCanvas();

                canvas.DrawRect(0, 0, Window.Size.Width, topAnchor, surface);

                sideBar.Draw(topAnchor);


                var textTop = topAnchor+25;
                canvas.DrawText(
                    Window.typedContent,
                    248, textTop,
                    TextPaint
                );

                var width = (int) TextPaint.MeasureText(Window.typedContent);

                carret.Move(
                    255 + width, topAnchor + 5
                );
                carret.Draw();
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