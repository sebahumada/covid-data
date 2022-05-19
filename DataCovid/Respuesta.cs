using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCovid
{
    public class Respuesta
    {
        public string UpdateAt { get; set; }
        public List<DatoCovid> Data { get; set; }
    }
}
