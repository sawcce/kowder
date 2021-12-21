namespace kowder.Input
{
    using System;
    using System.Drawing;

    using kowder;
    using SkiaSharp;
    using GLFW;

    class LabeledCheckBox : CheckBox
    {
        string label;
        public LabeledCheckBox(int x, int y, string label)
        {
            position.X = x;
            position.Y = y;

            this.label = label;

            Init();
        }

        public void Draw()
        {
            if (InBounds())
            {
                canvas.DrawRoundRect(
                    position.X, position.Y,
                    20, 20,
                    6, 6,
                    new SKPaint
                    {
                        Color = new SKColor(100, 100, 100, 255)
                    }
                );
            }
            else
            {
                canvas.DrawRoundRect(
                    position.X, position.Y,
                    20, 20,
                    6, 6,
                    new SKPaint
                    {
                        Color = new SKColor(60, 60, 60, 255)
                    }
                );
            }

            if (Checked)
            {
                canvas.DrawRoundRect(
                    position.X + 2, position.Y + 2,
                    16, 16,
                    5, 5,
                    new SKPaint
                    {
                        Color = new SKColor(100, 100, 160, 255)
                    }
                );
            }

            var textBounds = new SKRect();

            Themes.TextPaint.MeasureText(label, ref textBounds);
            var height = textBounds.Size.Height;

            canvas.DrawText(label, new SKPoint(position.X + 30, position.Y + 40 - height), Themes.InputLabelPaint);

        }

    }

    class CheckBox
    {
        public SKCanvas canvas;

        protected SKPaint hovered = new SKPaint
        {
            Color = new SKColor(100, 100, 100, 255)
        };
        protected SKPaint current = Themes.InputBackground;

        public bool Checked = false;
        public bool Hovered = false;

        public Point position;
        public CheckBox() { }
        public CheckBox(int x, int y)
        {
            position.X = x;
            position.Y = y;
            Init();
        }

        protected void Init()
        {
            kowder.Window.LeftMouseButtonPressed += delegate ()
            {
                if (InBounds())
                {
                    Checked = !Checked;
                }
            };
        }

        protected void Enable() 
        {
            Init();
        }

        public void Disable() 
        {
            kowder.Window.MouseButtonPressed -= delegate (object sender, MouseButton mb)
            {
                if (mb == MouseButton.Left && InBounds())
                {
                    Console.WriteLine("{0}", mb);
                    Checked = !Checked;
                }
            };
        }

        public void Draw()
        {
            if (InBounds())
            {
            }
            else
            {
                canvas.DrawRoundRect(
                    position.X, position.Y,
                    20, 20,
                    6, 6,
                    Themes.InputBackground
                );
            }

            canvas.DrawRoundRect(
                position.X, position.Y,
                20, 20,
                6, 6,
                hovered
            );

            if (Checked)
            {
                canvas.DrawRoundRect(
                    position.X + 2, position.Y + 2,
                    16, 16,
                    5, 5,
                    new SKPaint
                    {
                        Color = new SKColor(100, 100, 160, 255)
                    }
                );
            }
        }

        public bool InBounds()
        {
            return kowder.Window.IsCursorInBounds(position.X, position.X + 20, position.Y, position.Y + 20);
        }
    }
}