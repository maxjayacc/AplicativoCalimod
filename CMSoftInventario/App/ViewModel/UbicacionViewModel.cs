using Android.App;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using CMSoftInventario.App.Adapter;
using CMSoftInventario.App.Model;
using CMSoftInventario.App.Util;
using System;
using System.Collections.Generic;

namespace CMSoftInventario.App.ViewModel
{
    public class UbicacionViewModel
    {
        private MemoryData memoryData;
        public async void CargarListado(Activity context, Android.Support.V4.App.Fragment fragment)
        {
            memoryData = MemoryData.GetInstance(context);
            List<Ubicacion> lista = new List<Ubicacion>();
            int idTerminal = memoryData.GetData(Constante.CMIdterminal);
            int idUsuario = memoryData.GetData(Constante.CMIdUsuario);
            Parametro parametro = Globals.Parametro;

            if (idUsuario > 0 && idTerminal > 0 && parametro.IdInventario > 0)
            {
                View view = fragment.View;
                if (new Networks(context).VerificaConexion())
                {
                    var progressDialog = ProgressDialog.Show(context, context.GetString(Resource.String.progress_titulo), context.GetString(Resource.String.progress_mensaje), true);
                    try
                    {
                        Proxy.ProxyGestorRx gestorRx = new Proxy.ProxyGestorRx();
                        lista = await gestorRx.ObtenerListaUbicacion(idUsuario, idTerminal, parametro.IdInventario);

                        TextView tvCompania = view.FindViewById<TextView>(Resource.Id.ubi_compania);
                        TextView tvInventario = view.FindViewById<TextView>(Resource.Id.ubi_inventario);
                        TextView tvAlmacen = view.FindViewById<TextView>(Resource.Id.ubi_almacen);
                        tvCompania.Text = parametro.CompaniaDescripcion;
                        tvInventario.Text = parametro.NombreInventario;
                        tvAlmacen.Text = parametro.AlmacenDescripcion;

                        RecyclerView rvUbicacion = view.FindViewById<RecyclerView>(Resource.Id.recyclerViewUbi);
                        RecyclerView.LayoutManager rvUbicacionLayout = new LinearLayoutManager(context, LinearLayoutManager.Vertical, false);
                        rvUbicacion.SetLayoutManager(rvUbicacionLayout);
                        RecyclerView.Adapter rvUbicacionAdapter = new UbicacionAdapter(context, lista, rvUbicacion);
                        rvUbicacion.SetAdapter(rvUbicacionAdapter);
                    }
                    catch (ApplicationException) { Snackbar.Make(view, Resource.String.app_error, Snackbar.LengthLong).Show(); }
                    catch (Exception) { Snackbar.Make(view, Resource.String.app_error, Snackbar.LengthLong).Show(); }
                    finally { progressDialog.Hide(); }
                }
                else { Snackbar.Make(view, Resource.String.app_network, Snackbar.LengthLong).Show(); }
            }

        }
    }
}