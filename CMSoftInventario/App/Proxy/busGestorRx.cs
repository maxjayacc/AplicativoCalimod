using CMSoftInventario.App.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMSoftInventario.App.Proxy
{
    public class ProxyGestorRx
    {
        public async Task<Equipo> ObtenerEquipo(string dispositivo)
        {
            Equipo equipo = new Equipo();
            try
            {
                string resul = await Solicitud.ObtenerSolicitud<Equipo>(string.Concat(@"GetEquipo/", dispositivo), null);
                if (!string.IsNullOrEmpty(resul)) { equipo = Util.Utilitarios.DeserializarRest<Equipo>(resul); }
                else { throw new ApplicationException("Se produjo un error al obtener los datos"); }
            }
            catch (ApplicationException ex) { throw ex; }
            catch (Exception ex) { throw ex; }
            return equipo;
        }

        public async Task<Usuario> ObtenerAcceso(Login login)
        {
            Usuario resulUser = null;
            try
            {
                string resul = await Solicitud.ObtenerSolicitud(@"LoginUsuario", login, true);
                if (!string.IsNullOrEmpty(resul)) { resulUser = Util.Utilitarios.DeserializarRest<Usuario>(resul); }
                else { throw new ApplicationException("Se produjo un error al obtener los datos"); }
            }
            catch (ApplicationException ex) { throw ex; }
            catch (Exception ex) { throw ex; }
            return resulUser;
        }

        public async Task<List<Inventario>> ObtenerListaInventario(int idUsuario, int idTerminal)
        {
            List<Inventario> lista = new List<Inventario>();
            try
            {
                string resul = await Solicitud.ObtenerSolicitud<Inventario>(string.Concat(@"GetListadoDeInventario/", idUsuario.ToString(), "/", idTerminal.ToString()), null);
                if (!string.IsNullOrEmpty(resul)) { lista = Util.Utilitarios.DeserializarRest<List<Inventario>>(resul); }
                else { throw new ApplicationException("Se produjo un error al obtener los datos"); }
            }
            catch (ApplicationException ex) { throw ex; }
            catch (Exception ex) { throw ex; }
            return lista;
        }

        public async Task<List<Ubicacion>> ObtenerListaUbicacion(int idUsuario, int idTerminal, int idInventario)
        {
            List<Ubicacion> lista = new List<Ubicacion>();
            try
            {
                string resul = await Solicitud.ObtenerSolicitud<Ubicacion>(string.Concat(@"GetListadoDeUbicaciones/", idUsuario.ToString(), "/", idTerminal.ToString(), "/", idInventario.ToString()), null);
                if (!string.IsNullOrEmpty(resul)) { lista = Util.Utilitarios.DeserializarRest<List<Ubicacion>>(resul); }
                else { throw new ApplicationException("Se produjo un error al obtener los datos"); }
            }
            catch (ApplicationException ex) { throw ex; }
            catch (Exception ex) { throw ex; }
            return lista;
        }

        public async Task<Confirmacion> ConfirmarUbicacion(int idUsuario, int idTerminal, int idInventario, string codigo)
        {
            Confirmacion objeto = new Confirmacion();
            try
            {
                string resul = await Solicitud.ObtenerSolicitud<Confirmacion>(string.Concat(@"ConfirmUbicacionAsignada/", idUsuario.ToString(), "/", idTerminal.ToString(), "/", idInventario.ToString(), "/", codigo), null);
                if (!string.IsNullOrEmpty(resul)) { objeto = Util.Utilitarios.DeserializarRest<Confirmacion>(resul); }
                else { throw new ApplicationException("Se produjo un error al obtener los datos"); }
            }
            catch (ApplicationException ex) { throw ex; }
            catch (Exception ex) { throw ex; }
            return objeto;
        }

        public async Task<List<ItemReconteo>> ObtenerListaItemReconteo(int idAsignacionUsuario)
        {
            List<ItemReconteo> objeto = new List<ItemReconteo>();
            try
            {
                string resul = await Solicitud.ObtenerSolicitud<Confirmacion>(string.Concat(@"GetItemsDeAsignacion/", idAsignacionUsuario.ToString()), null);
                if (!string.IsNullOrEmpty(resul)) { objeto = Util.Utilitarios.DeserializarRest<List<ItemReconteo>>(resul); }
                else { throw new ApplicationException("Se produjo un error al obtener los datos"); }
            }
            catch (ApplicationException ex) { throw ex; }
            catch (Exception ex) { throw ex; }
            return objeto;
        }

        public async Task<Totales> ObtenerTotalesTomas(int idAsignacionUsuario)
        {
            Totales objeto = new Totales();
            try
            {
                string resul = await Solicitud.ObtenerSolicitud<Confirmacion>(string.Concat(@"GetListaTomasTotales/", idAsignacionUsuario.ToString()), null);
                if (!string.IsNullOrEmpty(resul)) { objeto = Util.Utilitarios.DeserializarRest<Totales>(resul); }
                else { throw new ApplicationException("Se produjo un error al obtener los datos"); }
            }
            catch (ApplicationException ex) { throw ex; }
            catch (Exception ex) { throw ex; }
            return objeto;
        }

    }
}