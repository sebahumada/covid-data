﻿using System;
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
        public static List<string[]>? datosActivosPorComuna;

        public static List<string[]>? datosFallecidosPorComunaYTotal;

        public static List<string[]>? datosFallecidosPorRegion;

        public static List<string[]>? datosPositividadNacional;

        public static string[] codigos = { "15", "01", "02", "03", "04", "05", "13", "06", "07", "16", "08", "09", "14", "10", "11", "12" };

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


        



        public async Task<List<string[]>> GetDataFallecidosPorComunaYTotales()
        {
            if(datosFallecidosPorComunaYTotal !=null && datosFallecidosPorComunaYTotal.Count > 0)
            {
                return datosFallecidosPorComunaYTotal;
            }

            string url = "https://raw.githubusercontent.com/MinCiencia/Datos-COVID19/master/output/producto38/CasosFallecidosPorComuna.csv";
            datosFallecidosPorComunaYTotal = await GetDataAsync(url);

            var primeraFila = datosFallecidosPorComunaYTotal[0];

            UpdatedAt updated = new UpdatedAt();
            updated.UpdatedDate = primeraFila[primeraFila.Length - 1];
            updated.ProcessDate = DateTime.Now;



            var path = @"C:\Proyectos\Covid\TransformData\Output\";
            var jsonUpdateAt = JsonSerializer.Serialize(updated);

            File.WriteAllText(path + "dataFallecidosComunaUpdateAt.json", jsonUpdateAt);

            return datosFallecidosPorComunaYTotal;

        }

        public async Task<List<string[]>> GetDataPositividadNacionalDiaria()
        {
            if(datosPositividadNacional!=null && datosPositividadNacional.Count > 0)
            {
                return datosPositividadNacional;
            }

            string url = "https://raw.githubusercontent.com/MinCiencia/Datos-COVID19/master/output/producto49/Positividad_Diaria_Media.csv";
            datosPositividadNacional = await GetDataAsync(url);

            var primeraFila = datosPositividadNacional[0];

            UpdatedAt updated = new UpdatedAt();
            updated.UpdatedDate = primeraFila[primeraFila.Length - 1];
            updated.ProcessDate = DateTime.Now;



            var path = @"C:\Proyectos\Covid\TransformData\Output\";
            var jsonUpdateAt = JsonSerializer.Serialize(updated);

            File.WriteAllText(path + "dataPositividadNacionalDiariaUpdatedAt.json", jsonUpdateAt);

            return datosPositividadNacional;

        }


        public async Task GetPositividadNacionalDiaria()
        {
            var positividad = await GetDataPositividadNacionalDiaria();

            var primeraFila = positividad[0];

            PotividadNacional positividadNacional = new PotividadNacional();
            positividadNacional.UpdatedAt = primeraFila[primeraFila.Length - 1];

            List<FechaValorDec> listaFechaValorDec = new List<FechaValorDec>();

            var filaPositividad = positividad[4];

            for(int i = 1; i < filaPositividad.Length; i++)
            {
                FechaValorDec fv = new FechaValorDec();
                fv.Fecha = primeraFila[i];
                fv.Valor = ProcesarTextoDec(filaPositividad[i]);

                listaFechaValorDec.Add(fv);
            }

            positividadNacional.Lista = listaFechaValorDec;

            var path = @"C:\Proyectos\Covid\TransformData\Output\";
            var jsonUpdateAt = JsonSerializer.Serialize(positividadNacional);

            File.WriteAllText(path + "dataPositividadNacionalDiaria.json", jsonUpdateAt);

        }

        public async Task GetTotalFallecidosPorRegion()
        {
            var totalesComuna = await GetDataFallecidosPorComunaYTotales();

            var primeraFila = totalesComuna[0];
            RespuestaTotalRegion respuestaTotalRegion = new RespuestaTotalRegion();
            respuestaTotalRegion.UpdatedAt = primeraFila[primeraFila.Length - 1];

            List<RegionTotal> listaRegionTotal = new List<RegionTotal>();

            for (int i = 1; i < totalesComuna.Count; i++)
            {
                var fila = totalesComuna[i];

                if (fila[2].Equals("Total"))
                {
                    RegionTotal comunaTotal = new RegionTotal();
                    comunaTotal.Region = fila[1];
                    comunaTotal.Total = ProcesarTexto(fila[fila.Length - 1]);
                    listaRegionTotal.Add(comunaTotal);
                }
            }

            respuestaTotalRegion.Lista = listaRegionTotal;

            var path = @"C:\Proyectos\Covid\TransformData\Output\";
            var json = JsonSerializer.Serialize(respuestaTotalRegion);

            File.WriteAllText(path + "dataTotalesFallecidosRegion.json", json);
        }

        public async Task GetTotalFallecidosPorComuna()
        {
            var totalesComuna = await GetDataFallecidosPorComunaYTotales();

            var primeraFila = totalesComuna[0];
            RespuestaTotalComuna respuestaTotalComuna = new RespuestaTotalComuna();
            respuestaTotalComuna.UpdatedAt = primeraFila[primeraFila.Length - 1];

            List<ComunaTotal> listaComunaTotal = new List<ComunaTotal>();

            for(int i = 1; i < totalesComuna.Count; i++)
            {
                var fila = totalesComuna[i];

                if (!string.IsNullOrEmpty(fila[3]))
                {
                    ComunaTotal comunaTotal = new ComunaTotal();
                    comunaTotal.Comuna = fila[3];
                    comunaTotal.Total = ProcesarTexto(fila[fila.Length - 1]);
                    listaComunaTotal.Add(comunaTotal);
                }
            }

            respuestaTotalComuna.Lista = listaComunaTotal;

            var path = @"C:\Proyectos\Covid\TransformData\Output\";
            var json = JsonSerializer.Serialize(respuestaTotalComuna);

            File.WriteAllText(path + "dataTotalesFallecidosComuna.json", json);



        }

        public async Task GetTotalesPorComuna()
        {
            string url = "https://raw.githubusercontent.com/MinCiencia/Datos-COVID19/master/output/producto1/Covid-19.csv";
            var totalesComuna = await GetDataAsync(url);
            var primeraFila = totalesComuna[0];

            var totalColumnas = primeraFila.Length-1;

            RespuestaTotalComuna respuestaTotalComuna = new RespuestaTotalComuna();
            respuestaTotalComuna.UpdatedAt = primeraFila[totalColumnas - 1];

            List<ComunaTotal> lista = new List<ComunaTotal>();

            for(int i=1;i< totalesComuna.Count; i++)
            {
                var fila = totalesComuna[i];

                if (!string.IsNullOrEmpty(fila[3]))
                {
                    ComunaTotal comunaTotal = new ComunaTotal();
                    comunaTotal.Comuna = fila[3];
                    comunaTotal.Total = ProcesarTexto(fila[totalColumnas - 1]);
                    lista.Add(comunaTotal);
                }
                    

            }

            respuestaTotalComuna.Lista = lista;

            var path = @"C:\Proyectos\Covid\TransformData\Output\";
            var json = JsonSerializer.Serialize(respuestaTotalComuna);

            File.WriteAllText(path + "dataTotalesComuna.json", json);
        }


        public async Task GetTotalesPorRegion()
        {
            string url = "https://raw.githubusercontent.com/MinCiencia/Datos-COVID19/master/output/producto3/CasosTotalesCumulativo.csv";
            var totalesRegion = await GetDataAsync(url);

            //string[] codigos = { "15", "01", "02", "03", "04", "05", "13", "06", "07", "16", "08", "09", "14", "10", "11", "12" };
            

            var primeraFila = totalesRegion[0];

            var totalColumnas = primeraFila.Length;
            RespuestaTotalRegion respuestaTotalRegion = new RespuestaTotalRegion();
            respuestaTotalRegion.UpdatedAt = primeraFila[totalColumnas - 1];

            List<RegionTotal> lista = new List<RegionTotal>();

            for (int i=1;i< totalesRegion.Count - 1; i++)
            {
                var fila = totalesRegion[i];
                RegionTotal regionTotal = new RegionTotal();
                regionTotal.Region = codigos[i - 1];
                regionTotal.Total = ProcesarTexto(fila[totalColumnas - 1]);

                lista.Add(regionTotal);

            }

            respuestaTotalRegion.Lista = lista;
            var path = @"C:\Proyectos\Covid\TransformData\Output\";
            var json = JsonSerializer.Serialize(respuestaTotalRegion);

            File.WriteAllText(path + "dataTotalesRegion.json", json);           

        }



        public async Task<List<string[]>> GetDataActivosPorComuna()
        {
            if (datosActivosPorComuna != null && datosActivosPorComuna.Count > 0)
            {
                return datosActivosPorComuna;
            }


            string url = "https://raw.githubusercontent.com/MinCiencia/Datos-COVID19/master/output/producto19/CasosActivosPorComuna.csv";
            datosActivosPorComuna = await GetDataAsync(url);


            var primeraFila = datosActivosPorComuna[0];

            UpdatedAt updated = new UpdatedAt();
            updated.UpdatedDate = primeraFila[primeraFila.Length - 1];
            updated.ProcessDate = DateTime.Now;


           
            var path = @"C:\Proyectos\Covid\TransformData\Output\"; 
            var jsonUpdateAt = JsonSerializer.Serialize(updated);

            File.WriteAllText(path + "dataActivosComunaUpdateAt.json", jsonUpdateAt);

            return datosActivosPorComuna;
        }



        public async Task GetRegionComuna()
        {
            var listado = await GetDataActivosPorComuna();

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
        public async Task GetActivosPorComuna()
        {
            var listado = await GetDataActivosPorComuna();

            var primeraFila = listado[0];

            RespuestaActivosComuna respuestaActivosComuna = new RespuestaActivosComuna();
            respuestaActivosComuna.UpdatedAt = primeraFila[primeraFila.Length - 1];

            respuestaActivosComuna.Fechas = primeraFila.Skip(5).ToList();


            List<ActivosComuna> listaActivosComuna = new List<ActivosComuna>();

            for (int i = 1; i < listado.Count; i++)
            {
                var fila = listado[i];

                if (!string.IsNullOrEmpty(fila[3]))
                {
                    ActivosComuna activos = new ActivosComuna();
                    activos.C = fila[3];

                    List<int>listadoFechaValor = new List<int>();
                    for (int j = 5; j < fila.Length; j++)
                    {
                        //FechaValorComuna fechaValor = new FechaValorComuna();
                        //fechaValor.F = primeraFila[j];
                        //fechaValor.V = ProcesarTexto(fila[j]);

                        listadoFechaValor.Add(ProcesarTexto(fila[j]));

                        //listadoFechaValor.Add(fechaValor);
                    }

                    activos.D = listadoFechaValor;
                    listaActivosComuna.Add(activos);
                }

            }

            respuestaActivosComuna.Lista = listaActivosComuna;

            string json = JsonSerializer.Serialize(respuestaActivosComuna);
            string path = @"C:\Proyectos\Covid\TransformData\Output\";
            File.WriteAllText(path + "dataActivosComuna.json", json);
        }


        public async Task GetActivosRegionales()
        {
            var listado = await GetDataActivosPorComuna();

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
                    activos.Poblacion = ProcesarTexto(fila[4]);


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

            List<FechaValor> listaActivosNacional = new List<FechaValor>();

            for(int i = 5; i < primeraFila.Length; i++)
            {
                FechaValor fechaValor = new FechaValor();
                fechaValor.Fecha = primeraFila[i];

                int num = 0;
                foreach(var reg in listaActivosRegion)
                {
                    var data = reg.Data;

                    foreach(var fechaData in data)
                    {
                        if (fechaData.Fecha.Equals(primeraFila[i]))
                        {
                            num += fechaData.Valor;
                        }
                    }
                }

                fechaValor.Valor = num;
                listaActivosNacional.Add(fechaValor);
                
            }

            ActivosNacional activosNacional = new ActivosNacional();
            activosNacional.UpdatedAt = primeraFila[primeraFila.Length - 1];
            activosNacional.Lista = listaActivosNacional;

            string json = JsonSerializer.Serialize(respuestaActivosRegion);
            string path = @"C:\Proyectos\Covid\TransformData\Output\";
            File.WriteAllText(path + "dataActivosRegion.json", json);

            string jsonNacional = JsonSerializer.Serialize(activosNacional);            
            File.WriteAllText(path + "dataActivosNacional.json", jsonNacional);
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

        public static decimal ProcesarTextoDec(string valor)
        {
            if (string.IsNullOrEmpty(valor)) return 0;

            return Math.Round((Convert.ToDecimal(valor.Replace(".",",")))*100,2);
        }

        public static string ShortDate(string valor)
        {
            if (string.IsNullOrEmpty(valor)) return string.Empty;

            return valor.Replace("-",string.Empty);
        }


    }
}
