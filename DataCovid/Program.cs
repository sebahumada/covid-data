using System;

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
            x.GetTotalesPorRegion().Wait();
            x.GetTotalesPorComuna().Wait();
            x.GetTotalFallecidosPorComuna().Wait();
            x.GetTotalFallecidosPorRegion().Wait();
            x.GetPositividadNacionalDiaria().Wait();

            Console.WriteLine("FIN PROCESO: " + DateTime.Now.ToString());
        }
    }
}