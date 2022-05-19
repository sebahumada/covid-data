using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataCovid
{
    public class ExtractData
    {
        public async Task GetDataAsync()
        {

            string url = "https://raw.githubusercontent.com/MinCiencia/Datos-COVID19/master/output/producto19/CasosActivosPorComuna.csv";
            HttpClient client = new HttpClient();
            byte[] buffer = await client.GetByteArrayAsync(url);
            Stream stream = new MemoryStream(buffer);
            List<string[]> listado = new List<string[]>();
            using (var reader = new StreamReader(stream))
            {
                while (reader.Peek() >= 0)
                {
                    string? fila=reader.ReadLine();
                    if(!string.IsNullOrEmpty(fila))
                    {
                        string[] filaSeparada = fila.Split(',');
                        listado.Add(filaSeparada);
                    }                    
                }
            }

            var primeraFila = listado[0];

            List<DatoCovid> listadoCovid = new List<DatoCovid>();
            

            for (int i = 1; i< listado.Count; i++)
            {
                var fila = listado[i];
                DatoCovid dc = new DatoCovid();

                //dc.Region = fila[0];
                //dc.Reg = fila[1];
                //dc.Comuna = fila[2];
                dc.Comuna = fila[3];
                //dc.Poblacion = ProcesarTexto(fila[4]);
                //Recorrido dentro de la fila
                List<int> listaFechaCantidad = new List<int>();
                for (int j = 5; j < fila.Length; j++)
                {
                    //FechaCantidad fc = new FechaCantidad();
                    //fc.Dt = primeraFila[j];
                    //fc.Num = ProcesarTexto(fila[j]);
                    listaFechaCantidad.Add(ProcesarTexto(fila[j]));
                }

                dc.Casos = listaFechaCantidad;

                listadoCovid.Add(dc);
            }
            
            Respuesta r=new Respuesta();
            r.UpdateAt = primeraFila[primeraFila.Length-1];
            r.Data = listadoCovid;
            var json = JsonSerializer.Serialize(r);
            var path = @"C:\Proyectos\Covid\TransformData\Output\";
            File.WriteAllText(path+"data.json", json);

            

        }


        public static int ProcesarTexto(string valor)
        {
            if (string.IsNullOrEmpty(valor)) return 0;

            return Convert.ToInt32(valor.Replace(".0", string.Empty));
        }

        public static string ShortDate(string valor)
        {
            if (string.IsNullOrEmpty(valor)) return string.Empty;

            return valor.Replace("-",string.Empty);
        }


    }
}
