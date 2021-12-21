namespace kowder.Input
{
    using System;
    using GLFW;
    using System.Drawing;
    using System.Collections.Generic;
    using SkiaSharp;

    class DropDownChoice
    {
        public SKCanvas canvas;
        public Point position;
        public int width = 100;

        private static SKPaint TextPaint = new SKPaint
        {
            Color = new SKColor(200, 200, 200, 255),
            TextSize = 19
        };

        private static SKPaint SelectedTextPaint = new SKPaint
        {
            Color = new SKColor(100, 100, 200, 255),
            TextSize = 20
        };

        private int currentIndex;
        private List<string> items;
        private bool Opened = false;
        private float OpenedState = 0;
        private SKImage listImage;

        protected SKPaint color = new SKPaint
        {
            Color = new SKColor(60, 60, 60, 255)
        };

        public DropDownChoice(int currentIndex, List<string> items)
        {
            this.currentIndex = currentIndex;
            this.items = items;
            listImage = GenerateListImage();
        }

        public void Enable()
        {
            kowder.Window.LeftMouseButtonPressed += delegate ()
            {
                if (InBounds())
                {
                    Opened = !Opened;
                }

                if (Opened)
                {
                    var (inBounds, index) = kowder.Window.SmartCursorBoundsVertical(
                        position.X, position.X + width,
                        position.Y + 27, 26, items.Count);

                    if (!inBounds) return;

                    currentIndex = index;
                    listImage = GenerateListImage();
                }
            };
        }

        public bool InBounds()
        {
            return kowder.Window.IsCursorInBounds(position.X, position.X + width, position.Y, position.Y + 25);
        }

        public SKImage GenerateListImage()
        {
            var info = new SKImageInfo(width, items.Count * 26);
            using (var surface = SKSurface.Create(info))
            {
                int currentY = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    string item = items[i];

                    var paint = (items[currentIndex] == item) ? SelectedTextPaint : TextPaint;

                    surface.Canvas.DrawText(item, new SKPoint(10, currentY + 20), paint);
                    currentY += 26;
                }
                return surface.Snapshot();
            }
        }


        public void Draw()
        {
            canvas.DrawRoundRect(
                position.X,
                position.Y,
                width, 25,
                6, 6,
                color
            );

            canvas.DrawText(
                items[currentIndex],
                new SKPoint(position.X + 10, position.Y + 20),
                new SKPaint
                {
                    TextSize = 20,
                    Typeface = Themes.typeface,
                    Color = new SKColor(255, 255, 255, 255)
                }
            );

            if (OpenedState > 0.1f)
            {
                var heightOffset = (OpenedState - 1) * items.Count * 26;

                canvas.Save();
                canvas.ClipRect(new SKRect(position.X, position.Y + 27, position.X + width, position.Y + 27 + (items.Count * 26) + 10), SKClipOperation.Intersect);
                canvas.DrawRoundRect(
                    position.X,
                    position.Y + 27 + heightOffset,
                    width, items.Count * 26,
                    6, 6,
                    Themes.InputBackground
                );
                canvas.DrawImage(listImage,
                    position.X,
                    position.Y + 27 + heightOffset);
                canvas.Restore();

            }
            if (OpenedState < 1 && Opened)
                OpenedState = Interpolation.lerp(OpenedState, 1, 0.1f);
            if (OpenedState > 0 && !Opened)
                OpenedState = Interpolation.lerp(OpenedState, 0, 0.1f);
        }
    }
}