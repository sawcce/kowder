namespace kowder
{
    using SkiaSharp;
    using Svg.Skia;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading;

    class SideBar
    {
        ActionBar actionBar = new ActionBar();
        public SideBar() { }

        public void Draw(int topAnchor)
        {
            var canvas = Window.GetCanvas();

            canvas.DrawRect(0, topAnchor + 1, 248, Window.Size.Height - 30, Program.surface);

            actionBar.Draw(topAnchor);
        }
    }

    class ActionBar
    {
        public List<SKPicture> icons = new List<SKPicture>();
        private SKPaint iconPaint = new SKPaint { Color = SKColor.Parse("#aaa") };
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

            int currentX = topAnchor + gap;
            for (int i = 0; i < icons.Count; i ++)
            {
                var mousePos = Window.lastMousePosition;

                var inBounds = Window.IsCursorInBounds(10, 58, currentX - gap, currentX + 42);
                
                if (inBounds)
                {
                    canvas.DrawRoundRect(
                        new SKRoundRect(
                            new SKRect(10, currentX - gap, 58, currentX + 42),
                            15
                        ),
                        iconPaint
                    );
                }

                /* The x offset of the action bar is 10,
                the size of the logo is 30 * 30 and,
                since the width of the actionbar is 48,
                (48 - 30) / 2 = 9
                10 + 9 = 19  */

                canvas.DrawPicture(icons[i], 19, currentX);
                currentX = topAnchor + gap + height*(i+1); 

            }
        }

        public void AddIcon()
        {

        }
    }
}