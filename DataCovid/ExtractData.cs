using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using DataCovid.Resumen;

namespace DataCovid
{
    public class ExtractData
    {

        public async Task<List<string[]>> GetDataAsync(string url)
        {
            HttpClient client = new HttpClient();
            byte[] buffer = await client.GetByteArrayAsync(url);
            Stream stream = new MemoryStream(buffer);
            List<string[]> listado = new List<string[]>();
            using (var reader = new StreamReader(stream))
            {
                while (reader.Peek() >= 0)
                {
                    string? fila = reader.ReadLine();
                    if (!string.IsNullOrEmpty(fila))
                    {
                        string[] filaSeparada = fila.Split(',');
                        listado.Add(filaSeparada);
                    }
                }
            }

            return listado;
        }

        public async Task GetRegionComuna()
        {
            string url = "https://raw.githubusercontent.com/MinCiencia/Datos-COVID19/master/output/producto19/CasosActivosPorComuna.csv";

            var listado = await GetDataAsync(url);

            List<RegionComuna> regionComunas = new List<RegionComuna>();

            for(int i=1;i< listado.Count; i++)
            {
                var fila = listado[i];
                RegionComuna regionComuna = new RegionComuna();
                regionComuna.Region = fila[0];
                regionComuna.CodRegion = fila[1];
                regionComuna.Comuna = fila[2];
                regionComuna.CodComuna = fila[3];
                regionComuna.Poblacion = ProcesarTexto(fila[4]);

                regionComunas.Add(regionComuna);
            }

            


            string json = JsonSerializer.Serialize(regionComunas);
            string path = @"C:\Proyectos\Covid\TransformData\Output\";
            File.WriteAllText(path + "dataRegionComuna.json", json);


            var quitaRepetidos = (from item in regionComunas
                                  group item by new { item.Region, item.CodRegion } into g
                                  select new Region()
                                  {
                                      CodRegion = g.Key.CodRegion,
                                      Nombre = g.Key.Region
                                  }
                                  ).ToList();


            string jsonRegiones = JsonSerializer.Serialize(quitaRepetidos);            
            File.WriteAllText(path + "dataListaRegiones.json", jsonRegiones);

        }


        //Activos Informe Epidemiologico
        public async Task GetActivosRegionales()
        {
            string url = "https://raw.githubusercontent.com/MinCiencia/Datos-COVID19/master/output/producto19/CasosActivosPorComuna.csv";

            var listado = await GetDataAsync(url);

            var primeraFila = listado[0];

            RespuestaActivosRegion respuestaActivosRegion = new RespuestaActivosRegion();

            respuestaActivosRegion.UpdatedAt = primeraFila[primeraFila.Length - 1];

            List<ActivosRegion> listaActivosRegion = new List<ActivosRegion>();


            for (int i = 1; i < listado.Count; i++)
            {
                var fila = listado[i];

                if (fila[2].Equals("Total"))
                {
                    ActivosRegion activos = new ActivosRegion();
                    activos.CodRegion = fila[1];

                    List<FechaValor> listadoFechaValor = new List<FechaValor>();
                    for (int j = 5; j < fila.Length; j++)
                    {
                        FechaValor fechaValor = new FechaValor();
                        fechaValor.Fecha = primeraFila[j];
                        fechaValor.Valor = ProcesarTexto(fila[j]);

                        listadoFechaValor.Add(fechaValor);
                    }

                    activos.Data = listadoFechaValor;
                    listaActivosRegion.Add(activos);
                }

            }

            respuestaActivosRegion.Lista = listaActivosRegion;

            string json = JsonSerializer.Serialize(respuestaActivosRegion);
            string path = @"C:\Proyectos\Covid\TransformData\Output\";
            File.WriteAllText(path + "dataActivosRegion.json", json);
        }



        public async Task GetDataActivos()
        {

            string url = "https://raw.githubusercontent.com/MinCiencia/Datos-COVID19/master/output/producto19/CasosActivosPorComuna.csv";

            var listado = await GetDataAsync(url);

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


        public async Task GetDataResumen()
        {
            string url = "https://raw.githubusercontent.com/MinCiencia/Datos-COVID19/master/output/producto5/TotalesNacionales.csv";
            var listado = await GetDataAsync(url);
            var primeraFila = listado[0];

            ResumenCovid resumenCovid = new ResumenCovid();
            resumenCovid.UpdatedAt = primeraFila[primeraFila.Length - 1];

            List<DatosResumen> datosResumen = new List<DatosResumen>();

            for(int i=1;i<listado.Count; i++)
            {
                DatosResumen datoResumen = new DatosResumen();
                var fila = listado[i];
                datoResumen.Item = fila[0];

                List<FechaValor> listadoFechaValor = new List<FechaValor>();
                for(int j = 1; j < fila.Length; j++)
                {
                    FechaValor fechaValor = new FechaValor();
                    fechaValor.Fecha = primeraFila[j];
                    fechaValor.Valor = ProcesarTexto(fila[j]);

                    listadoFechaValor.Add(fechaValor);
                }

                datoResumen.Cantidad = listadoFechaValor;

                datosResumen.Add(datoResumen);
            }

            List<DatosResumen> resumenAux = new List<DatosResumen>();

            var casosTotales = datosResumen[1];
            casosTotales.Cantidad.RemoveRange(0, casosTotales.Cantidad.Count-1);
            resumenAux.Add(casosTotales); //casos totales

            var fallecidos = (DatosResumen) datosResumen[3].Clone();
            var fallecidosDiario = (DatosResumen) datosResumen[3].Clone();
            var fallecidosDiarioCorregido = (DatosResumen)datosResumen[19].Clone();




            List<FechaValor> listFall  = new List<FechaValor>();
            
            //Dia del cambio de método para informar fallecidos
            string fechaCorreccionFallecidos = "2022-03-21";
            //Correccion
            string c1 = "2020-07-17";
            int cantc1 = 98;

            string c2 = "2020-06-07";
            int cantc2 = 96;

            for (int i = 0; i < fallecidosDiario.Cantidad.Count; i++)
            {
                if (i == 0)
                {
                    listFall.Add(fallecidosDiario.Cantidad[0]);
                }
                else
                {

                    //Se realiza corrección dicho dia para concordancia con datos oficiales
                    if (fallecidosDiario.Cantidad[i].Fecha.Equals(fechaCorreccionFallecidos))
                    {
                        var hoy = fallecidosDiarioCorregido.Cantidad[i].Valor;
                        var ayer = fallecidosDiario.Cantidad[i - 1].Valor;

                        var cantidad = hoy - ayer;

                        FechaValor fv = new FechaValor();
                        fv.Fecha = fallecidosDiario.Cantidad[i].Fecha;
                        fv.Valor = cantidad;

                        listFall.Add(fv);
                    }
                    else if (fallecidosDiario.Cantidad[i].Fecha.Equals(c1))
                    {
                        FechaValor fv = new FechaValor();
                        fv.Fecha = fallecidosDiario.Cantidad[i].Fecha;
                        fv.Valor = cantc1;

                        listFall.Add(fv);
                    }
                    else if (fallecidosDiario.Cantidad[i].Fecha.Equals(c2))
                    {
                        FechaValor fv = new FechaValor();
                        fv.Fecha = fallecidosDiario.Cantidad[i].Fecha;
                        fv.Valor = cantc2;

                        listFall.Add(fv);
                    }
                    else
                    {
                        var hoy = fallecidosDiario.Cantidad[i].Valor;
                        var ayer = fallecidosDiario.Cantidad[i - 1].Valor;

                        var cantidad = hoy - ayer;

                        FechaValor fv = new FechaValor();
                        fv.Fecha = fallecidosDiario.Cantidad[i].Fecha;
                        fv.Valor = cantidad;

                        listFall.Add(fv);
                    }
                }




            }
            

            fallecidosDiario.Item = fallecidosDiario.Item + " diario";
            fallecidosDiario.Cantidad = listFall;

            resumenAux.Add(fallecidosDiario);

            fallecidos.Cantidad.RemoveRange(0, fallecidos.Cantidad.Count - 1);
            resumenAux.Add(fallecidos);

            resumenAux.Add(datosResumen[4]); // casos activos
            resumenAux.Add(datosResumen[6]); // casos nuevos totales


            resumenCovid.Data = resumenAux;



            UpdatedAt updated = new UpdatedAt();
            updated.UpdatedDate = resumenCovid.UpdatedAt;
            updated.ProcessDate = DateTime.Now;


            var json = JsonSerializer.Serialize(resumenCovid);
            var path = @"C:\Proyectos\Covid\TransformData\Output\";
            File.WriteAllText(path + "dataResumen.json", json);

            var jsonUpdateAt = JsonSerializer.Serialize(updated);
            
            File.WriteAllText(path + "dataUpdateAt.json", jsonUpdateAt);
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
