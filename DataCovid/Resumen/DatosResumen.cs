using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCovid.Resumen
{
    public class DatosResumen
    {
        public string Item { get; set; }
        public List<FechaValor> Cantidad { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
