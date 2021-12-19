namespace kowder 
{
    using SkiaSharp;
    class KowderEditor {
        public static SKPaint surface = new SKPaint { Color = new SKColor(31, 31, 31, 255) };
        public static SKPaint bg = new SKPaint { Color = SKColor.Parse("#121212") };
        private static SideBar sideBar = new SideBar();
        private static Carret carret = new Carret(0, 0);
        public static int topAnchor = 35;
        public static SKCanvas canvas;
        public static void Init() {

            var TextPaint = new SKPaint { Color = new SKColor(255, 255, 255, 255), TextSize = 25 };
            TextPaint.Typeface = SKTypeface.FromFile("res/um.ttf");

            Window.SetRenderMethod(delegate ()
            {
                canvas.DrawRect(0, 0, Window.Size.Width, topAnchor, surface);

                sideBar.Draw(topAnchor);

                var textTop = topAnchor+25;
                canvas.DrawText(
                    Window.GetSize().ToString(),
                    248, textTop,
                    TextPaint
                );

                var width = (int) TextPaint.MeasureText(Window.typedContent);

                carret.Move(
                    255 + width, topAnchor + 5
                );
                carret.Draw();
            });
        }

        public static void SizeChanged() {
            sideBar.actionBar.GenerateIconsImage();
            sideBar.actionBar.canvas = Window.GetCanvas();
        }
    }
}