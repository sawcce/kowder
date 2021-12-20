namespace kowder
{
    using System;
    using System.Drawing;
    using System.Collections.Generic;
    using SkiaSharp;

    class DropDownChoice
    {
        public SKCanvas canvas;
        public Point position;

        private int currentIndex;
        private List<string> items;

        protected SKPaint color = new SKPaint
        {
            Color = new SKColor(60, 60, 60, 255)
        };

        public DropDownChoice(int currentIndex, List<string> items)
        {
            this.currentIndex = currentIndex;
            this.items = items;
        }

        public void Draw()
        {
            canvas.DrawRoundRect(
                position.X,
                position.Y,
                100, -25,
                5, 5,
                color
            );

            Console.WriteLine("{0} {1}",KeyboardLayouts.currentLayout, KeyboardLayouts.layoutsNames.Count);

            canvas.DrawText(
                items[currentIndex == -1 ? 0 : currentIndex],
                new SKPoint(position.X + 10, position.Y - 5),
                new SKPaint
                {
                    TextSize = 20,
                    Typeface = Themes.typeface,
                    Color = new SKColor(255, 255, 255, 255)
                }
            );
        }
    }
}