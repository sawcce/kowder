namespace kowder 
{
    using SkiaSharp;
    using System.Threading.Tasks;
    class KowderEditor {
        private static SideBar sideBar = new SideBar();
        private static Carret carret = new Carret(0, 0);
        public static int topAnchor = 35;
        public static SKCanvas canvas;
        public static void Init() {
            var src = @"
            in fragmentProcessor color_map;

            void main(float2 coord) {
                color = vec4(coord.x, coord.y, 0, 255);
            }";
            //using var effect = SKRuntimeEffect.Create(src, out var errorText);


            Window.SetRenderMethod(delegate ()
            {
                canvas.DrawRect(0, 0, Window.Size.Width, topAnchor, Themes.surface);

                sideBar.Draw(topAnchor);

                var textTop = topAnchor+25;
                canvas.DrawText(
                    Window.typedContent,
                    248, textTop,
                    Themes.TextPaint
                );

                var width = (int) Themes.TextPaint.MeasureText(Window.typedContent);

                carret.Move(
                    255 + width, topAnchor + 5
                );
                carret.Draw();
            });
        }

        public static void SizeChanged() {
            Task.Run(()=> sideBar.actionBar.GenerateIconsImage());
            sideBar.actionBar.canvas = Window.GetCanvas();
        }
    }
}