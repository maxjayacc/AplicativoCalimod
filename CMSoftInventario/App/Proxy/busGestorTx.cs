using CMSoftInventario.App.Model;
using System;
using System.Threading.Tasks;
namespace CMSoftInventario.App.Proxy
{
    public class ProxyGestorTx
    {

        public async Task<string> LecturaItemUnitario(Item item)
        {
            string res = string.Empty;
            try
            {
                string resul = await Solicitud.ObtenerSolicitud(@"LecturaItemUnitario", item, true);
                if (!string.IsNullOrEmpty(resul)) { res = Util.Utilitarios.DeserializarRest<string>(resul); }
                else { throw new ApplicationException("Se produjo un error al obtener los datos"); }
            }
            catch (ApplicationException ex) { throw ex; }
            catch (Exception ex) { throw ex; }
            return res;
        }

        public async Task<string> LecturaItemCaja(KitCaja kit)
        {
            string res = string.Empty;
            try
            {
                string resul = await Solicitud.ObtenerSolicitud(@"LecturaItemCaja", kit, true);
                if (!string.IsNullOrEmpty(resul)) { res = Util.Utilitarios.DeserializarRest<string>(resul); }
                else { throw new ApplicationException("Se produjo un error al obtener los datos"); }
            }
            catch (ApplicationException ex) { throw ex; }
            catch (Exception ex) { throw ex; }
            return res;
        }

        public async Task<string> LecturaItemKit(Kit kit)
        {
            string res = string.Empty;
            try
            {
                string resul = await Solicitud.ObtenerSolicitud(@"LecturaItemKit", kit, true);

                if (!string.IsNullOrEmpty(resul))
                {
                    res = Util.Utilitarios.DeserializarRest<string>(resul);
                    Task.WaitAll();
                    //toneGen1.Release();

                }
                else { throw new ApplicationException("Se produjo un error al obtener los datos"); }


                return res;
            }


            catch (ApplicationException ex) { throw ex; }
            catch (Exception ex) { throw ex; }

        }

        public async Task<ResulKit> CerrarLecturaItemKitUnitario(Kit kit)
        {
            ResulKit res = new ResulKit();
            try
            {
                string resul = await Solicitud.ObtenerSolicitud(@"CerrarLecturaItemKitUnitario", kit, true);
                if (!string.IsNullOrEmpty(resul)) { res = Util.Utilitarios.DeserializarRest<ResulKit>(resul); }
                else { throw new ApplicationException("Se produjo un error al obtener los datos"); }
            }
            catch (ApplicationException ex) { throw ex; }
            catch (Exception ex) { throw ex; }
            return res;
        }

        public async Task<string> BorrarLecturaItemUnitario(Item item)
        {
            string res = string.Empty;
            try
            {
                string resul = await Solicitud.ObtenerSolicitud(@"BorrarLecturaItemUnitario", item, true);
                if (!string.IsNullOrEmpty(resul)) { res = Util.Utilitarios.DeserializarRest<string>(resul); }
                else { throw new ApplicationException("Se produjo un error al obtener los datos"); }
            }
            catch (ApplicationException ex) { throw ex; }
            catch (Exception ex) { throw ex; }
            return res;
        }

        public async Task<string> BorrarLecturaItemKitUnitario(Kit kit)
        {
            string res = string.Empty;
            try
            {
                string resul = await Solicitud.ObtenerSolicitud(@"BorrarLecturaItemKitUnitario", kit, true);
                if (!string.IsNullOrEmpty(resul)) { res = Util.Utilitarios.DeserializarRest<string>(resul); }
                else { throw new ApplicationException("Se produjo un error al obtener los datos"); }
            }
            catch (ApplicationException ex) { throw ex; }
            catch (Exception ex) { throw ex; }
            return res;
        }

        public async Task<string> CerrarUbicacionLocal(UbicacionCerrar ubicacion)
        {

            string res = string.Empty;

            try
            {

                string resul = await Solicitud.ObtenerSolicitud(@"CerrarUbicacion_Local", ubicacion, true);
                if (!string.IsNullOrEmpty(resul)) { res = Util.Utilitarios.DeserializarRest<string>(resul); }
                else { throw new ApplicationException("Se produjo un error al obtener los datos"); }
            }
            catch (ApplicationException ex) { throw ex; }
            catch (Exception ex) { throw ex; }
            return res;
        }

        public async Task<string> MiddlewareCerrarInterface(UbicacionCerrarMiddleware ubicacion)
        {
            string res = string.Empty;
            try
            {
                string resul = await Solicitud.ObtenerSolicitud(@"Middleware_CerrarInterface", ubicacion, true, true);
                if (!string.IsNullOrEmpty(resul)) { res = Util.Utilitarios.DeserializarRest<string>(resul); }
                else { throw new ApplicationException("Se produjo un error al obtener los datos"); }
            }
            catch (ApplicationException ex) { throw ex; }
            catch (Exception ex) { throw ex; }
            return res;
        }

        public async Task<string> CerrarUbicacionItemLocal(ItemCerrar item)
        {
            string res = string.Empty;
            try
            {
                string resul = await Solicitud.ObtenerSolicitud(@"CerrarUbicacionItem_Local", item, true);
                if (!string.IsNullOrEmpty(resul)) { res = Util.Utilitarios.DeserializarRest<string>(resul); }
                else { throw new ApplicationException("Se produjo un error al obtener los datos"); }
            }
            catch (ApplicationException ex) { throw ex; }
            catch (Exception ex) { throw ex; }
            return res;
        }


    }
}