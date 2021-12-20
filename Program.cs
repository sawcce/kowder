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

            Startup.Init();
            
            Window.Init();
        }

    }
}