using CMSoftInventario.App.Configuration;
using CMSoftInventario.App.Model;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CMSoftInventario.App.Proxy
{
    public static class Solicitud
    {
        public static async Task<string> ObtenerSolicitud<T>(string router, T objeto, bool esPost = false, bool esMiddle = false)
        {
            string resultado = string.Empty;
            try
            {
                string baseUrl = string.Empty;
                string path = string.Empty;
                string uri = string.Empty;
                Config config = null;
                using (var cts = new CancellationTokenSource())
                {
                    config = await ConfigurationManager.Instance.GetAsync(cts.Token);
                    uri = string.Concat(config.BaseUrl, config.Path, router);
                }
                Equipo equipo = Globals.Equipo;
                if (equipo != null)
                {
                    if (equipo.IdEquipo > 0)
                    {
                        if (esMiddle && !string.IsNullOrEmpty(equipo.IpMiddleWare) && !string.IsNullOrEmpty(equipo.PathMiddleWare))
                        {
                            uri = string.Concat("http://", equipo.IpMiddleWare, equipo.PuertoMiddleWare == 0 ? string.Empty : string.Concat(":", equipo.PuertoMiddleWare), "/", equipo.PathMiddleWare, "/", router);

                        }
                        else if (!esMiddle && !string.IsNullOrEmpty(equipo.IpServicio) && !string.IsNullOrEmpty(equipo.PathServicio))
                        {
                            uri = string.Concat("http://", equipo.IpServicio, equipo.PuertoServicio == 0 ? string.Empty : string.Concat(":", equipo.PuertoServicio), "/", equipo.PathServicio, "/", router);
                        }
                    }
                }

                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = null;
                    uri = string.Concat(config.BaseUrl, config.Path, router);

                    if (esPost)
                    {
                        var json = Util.Utilitarios.SerializarRest(objeto);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        response = await client.PostAsync(uri, content);
                    }
                    else { response = await client.GetAsync(uri); }

                    // on error throw a exception  
                    //response.EnsureSuccessStatusCode();

                    resultado = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex) { throw ex; }
            return resultado;
        }

    }
}