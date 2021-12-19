namespace kowder
{
    using System;
    using SkiaSharp;
    using System.Threading.Tasks;

    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(() => Packages.Load());

            KowderEditor.Init();
            Window.Init();
        }

    }
}