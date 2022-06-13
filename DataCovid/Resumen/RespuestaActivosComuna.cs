using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCovid.Resumen
{
    public class RespuestaActivosComuna
    {
        public string UpdatedAt { get; set; }

        public List<string> Fechas { get; set; }
        public List<ActivosComuna> Lista { get; set; }
    }
}
