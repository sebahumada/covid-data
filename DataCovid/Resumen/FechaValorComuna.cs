using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCovid.Resumen
{
    public class FechaValorComuna
    {
        //public string F { get; set; }
        public int V { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
