namespace kowder
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using SkiaSharp;
    using kowder.Input;
    class Startup
    {
        public static SKPaint circle = new SKPaint { Color = new SKColor(255, 255, 255, 50) };
        public static SKCanvas canvas;
        public static LabeledCheckBox isFancyBGActivated = new LabeledCheckBox(100, 175, "Fancy background");
        public static DropDownChoice keyboardLayout = new DropDownChoice(KeyboardLayouts.layoutsNames.IndexOf(KeyboardLayouts.currentLayout), KeyboardLayouts.layoutsNames);
        public static float i = 0;

        public static void SetCanvas() 
        {
            isFancyBGActivated.canvas = canvas;
            keyboardLayout.canvas = canvas;
        }
        public static void Init()
        {
            keyboardLayout.position = new Point(100, 250);
            Window.SetRenderMethod(delegate ()
            {
                canvas.DrawRect(0, 0, Window.Size.Width, Window.Size.Height, Themes.surface);

                if(isFancyBGActivated.Checked) 
                {
                    DrawBG();
                }
                
                isFancyBGActivated.Draw();
                keyboardLayout.Draw();

                i += 0.01f;
                canvas.DrawText("Welcome Kowder!", new SKPoint(100, 100), Themes.Title);
                canvas.DrawText("Code, Learn, Edit", new SKPoint(100, 150), Themes.TextPaint);
            });

        }

        public static void DrawBG() {
                var mp = Window.lastMousePosition;
                var size = Window.Size;
                var cursorX = ((float)mp.X / (float)size.Width * 100) - 50;
                var cursorY = ((float)mp.Y / (float)size.Height * 100) - 50;

                for (float x = 0; x <= 9; x += 1)
                {
                    for (float y = 0; y <= 9; y += 1)
                    {
                        circle.Color = new SKColor((byte)(x * y * 5 * MathF.Abs(MathF.Sin(i))),(byte)(y * 5 * MathF.Abs(MathF.Sin(i))), (byte)(x*y), (byte) (MathF.Abs(MathF.Sin(i)) * 150 + 30));
                        
                        canvas.DrawCircle(
                            x * (size.Width / 11) + (size.Width / 11) + cursorX * ((x - 5) / 10) * (MathF.Sin(i) + 2) + ((MathF.Sin(i) * 10) - 5) + ((x*y) / 10)*MathF.Sin(i),
                            y * (size.Width / 11) + (size.Height / 11) + cursorY * ((x - 5) / 10) * (MathF.Sin(i) + 2) + ((MathF.Sin(i) * 10) - 5) + ((x*y) / 10)*MathF.Sin(i),
                            x*y/ 10 + 10, circle);
                    }

                }
        }
    }
}