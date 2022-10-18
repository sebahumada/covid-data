using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCovid.Resumen
{
    public class FechaValorDec
    {
        public string Fecha { get; set; }
        public decimal Valor { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
