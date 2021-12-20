namespace kowder
{
    using System;
    using SkiaSharp;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            Packages.Load();

            KowderEditor.Init();
            Window.SetRenderMethod(delegate ()
            {
                Window.GetCanvas().DrawRect(0,0,10,10, new SKPaint{
                    Color = new SKColor(235,125,10, 255)
                });
            });
            Window.Init();
        }

    }
}