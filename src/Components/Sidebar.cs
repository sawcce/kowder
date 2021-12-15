namespace kowder
{
    using SkiaSharp;
    using Svg.Skia;
    using System.Collections.Generic;
    using System.Drawing;

    class SideBar
    {
        ActionBar actionBar = new ActionBar();
        public SideBar() { }

        public void Draw(int topAnchor)
        {
            var canvas = Window.GetCanvas();

            canvas.DrawRect(0, topAnchor + 1, 48 + 200, Window.Size.Height - 30, Program.surface);

            actionBar.Draw(topAnchor);
        }
    }

    class ActionBar
    {
        public List<SKPicture> icons = new List<SKPicture>();
        public ActionBar()
        {
            var file = new SKSvg();
            icons.Add(file.Load("svg\\file.svg"));

            var packages = new SKSvg();
            icons.Add(packages.Load("svg\\boxes.svg"));
        }

        public void Draw(int topAnchor)
        {
            var canvas = Window.GetCanvas();

            topAnchor = topAnchor + 20;

            canvas.DrawRoundRect(
                new SKRoundRect(
                    new SKRect(10, topAnchor, 10 + 48, Window.Size.Height - 20),
                    15
                ),
                Program.bg
            );


            var gap = 12;
            var height = 54;
            var index = 0;

            for (int i = gap; i < icons.Count * (gap * 2 + 30); i += gap * 2 + 30)
            {
                var mousePos = Window.GetMousePosition().Value;

                var inBounds = Window.IsCursorInBounds(10, 58, topAnchor + i - gap, topAnchor + i + 42);
                
                if (inBounds)
                {
                    canvas.DrawRoundRect(
                        new SKRoundRect(
                            new SKRect(10, topAnchor + i - gap, 58, topAnchor + i + 42),
                            15
                        ),
                    new SKPaint { Color = SKColor.Parse("#aaa") });
                }

                /* The x offset of the action bar is 10,
                the size of the logo is 30 * 30 and,
                since the width of the actionbar is 48,
                (48 - 30) / 2 = 9
                10 + 9 = 19 */

                canvas.DrawPicture(icons[index], 19, topAnchor + i, null);
                index++;

            }
        }

        public void AddIcon()
        {

        }
    }
}