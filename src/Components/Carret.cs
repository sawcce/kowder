namespace kowder
{
    using System;
    using System.Drawing;
    using SkiaSharp;

    class Carret
    {
        protected Point position;
        protected float iterations = 0;
        protected byte Alpha = 0;
        protected SKPaint paint = new SKPaint
        {
            Color = new SKColor(255, 255, 255, 0)
        };

        public Carret(int x, int y)
        {
            position.X = x;
            position.Y = y;
        }

        public void Move(int x, int y)
        {
            position.X = Interpolation.lerp(position.X, x, 0.2f);
            position.Y = Interpolation.lerp(position.Y, y, 0.2f);
        }

        public void Draw()
        {

            Window.GetCanvas().DrawRect(
                position.X, position.Y,
                2, 25,
                paint
            );

            paint.Color = new SKColor(255, 255, 255, Alpha);
            Alpha = (byte)(MathF.Abs(MathF.Sin(iterations)) * 255);
            iterations += 0.01f;
        }

    }
}