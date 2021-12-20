namespace kowder
{
    using SkiaSharp;
    class Themes 
    {
        public static SKTypeface typeface;
        public static SKPaint surface = new SKPaint { Color = new SKColor(31, 31, 31, 255) };
        public static SKPaint white = new SKPaint { Color = new SKColor(255,255,255,255) };
        public static SKPaint bg = new SKPaint { Color = SKColor.Parse("#121212") };
        public static SKPaint Title = new SKPaint { Color = new SKColor(255, 255, 255, 255), TextSize = 40 };
        public static SKPaint TextPaint = new SKPaint { Color = new SKColor(255, 255, 255, 255), TextSize = 25 };
       
        public static SKPaint InputLabelPaint = new SKPaint { Color = new SKColor(120, 120, 120, 255), TextSize = 19};

        public static void Init() 
        {
            typeface = SKTypeface.FromFile("res/um.ttf");
            Title.Typeface = typeface;
            TextPaint.Typeface = typeface;
        }
    }
}