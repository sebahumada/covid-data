using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCovid.Resumen
{
    public class RespuestaTotalRegion
    {
        public string UpdatedAt { get; set; }

        public List<RegionTotal> Lista { get; set; }
    }
}
