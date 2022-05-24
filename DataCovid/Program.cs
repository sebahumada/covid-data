using System;

namespace DataCovid // Note: actual namespace depends on the project name.
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var x = new ExtractData();
            //x.GetDataActivos().Wait();
            x.GetDataResumen().Wait();
        }
    }
}