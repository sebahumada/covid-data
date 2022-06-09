using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCovid.Resumen
{
    public class ActivosRegion
    {
        public string CodRegion { get; set; }

        public int Poblacion { get; set; }
        public List<FechaValor> Data { get; set; }
    }
}
