namespace kowder
{
    using SkiaSharp;
    using Svg.Skia;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading.Tasks;
    using System;

    class SideBar
    {
        public ActionBar actionBar = new ActionBar();
        public SideBar() { }

        public void Draw(int topAnchor)
        {
            var canvas = Window.GetCanvas();

            canvas.DrawRect(0, topAnchor + 1, 248, Window.Size.Height - 30, KowderEditor.surface);

            actionBar.Draw(topAnchor);
        }
    }

    class ActionBar
    {
        public List<SKPicture> icons = new List<SKPicture>();
        private SKPaint iconPaint = new SKPaint { Color = SKColor.Parse("#aaa") };
        public SKCanvas canvas;
        private SKImage iconsImage;
        public ActionBar()
        {
            var file = new SKSvg();
            icons.Add(file.Load("svg\\file.svg"));

            var packages = new SKSvg();
            icons.Add(packages.Load("svg\\boxes.svg"));
            canvas = Window.GetCanvas();

            GenerateIconsImage();
        }

        /// <summary>
        /// For huge performance gains, the icons are actually
        /// being drawn before anything is rendered on a separate canvas
        /// and then stored in "iconsImage".  
        /// It drops cpu usage from ~15% to ~0.9-3.5%
        /// and significantly drops power usage in windows task manager
        /// compared to rendering the icons separately for each frame
        /// </summary>
        public void GenerateIconsImage()
        {
            // 75 : Top Anchor (35) + 20 (ActionBar top offsetY) + 20 (ActionBar bottom offsetY)
            var info = new SKImageInfo(48, Window.Size.Height - 75);

            var gap = 12;
            var height = 54;
            int currentY = gap;


            using (var surface = SKSurface.Create(info))
            {

                for (int i = 0; i < icons.Count; i++)
                {
                    surface.Canvas.DrawPicture(icons[i], 0, currentY);
                    currentY = gap + height * (i + 1);
                }

                iconsImage = surface.Snapshot();
            }

        }


        public void Draw(int topAnchor)
        {
            topAnchor = KowderEditor.topAnchor + 20;

            canvas.DrawRoundRect(
                new SKRoundRect(
                    new SKRect(10, topAnchor, 10 + 48, Window.Size.Height - 20),
                    15
                ),
                KowderEditor.bg
            );


            var gap = 12;
            var height = 54;

            int currentY = topAnchor + gap;
            for (int i = 0; i < icons.Count; i++)
            {
                var inBounds = Window.IsCursorInBounds(10, 58, currentY - gap, currentY + 42);

                if (inBounds)
                {
                    canvas.DrawRoundRect(
                        new SKRoundRect(
                            new SKRect(10, currentY - gap, 58, currentY + 42),
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

                currentY = topAnchor + gap + height * (i + 1);
                canvas.DrawImage(iconsImage, 19, topAnchor);
            }

        }

        public void AddIcon()
        {

        }
    }
}