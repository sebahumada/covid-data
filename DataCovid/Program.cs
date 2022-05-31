﻿using System;

namespace DataCovid // Note: actual namespace depends on the project name.
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("COVID INFO CL - Inicio: "+DateTime.Now.ToString());
            var x = new ExtractData();
            
            x.GetDataResumen().Wait();
            x.GetActivosRegionales().Wait();
            x.GetRegionComuna().Wait();
            x.GetActivosPorComuna().Wait();

            Console.WriteLine("FIN PROCESO: " + DateTime.Now.ToString());
        }
    }
}