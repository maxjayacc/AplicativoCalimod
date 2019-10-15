using Android.App;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using CMSoftInventario.App.Adapter;
using CMSoftInventario.App.Model;
using CMSoftInventario.App.Util;
using System;
using System.Collections.Generic;

namespace CMSoftInventario.App.ViewModel
{
    public class InventarioViewModel
    {
        private MemoryData memoryData;

        public async void CargarListado(Activity context, View view, Android.Support.V4.App.Fragment fragment = null)
        {
            memoryData = MemoryData.GetInstance(context);
            List<Inventario> lista = new List<Inventario>();
            int idTerminal = memoryData.GetData(Constante.CMIdterminal);
            int idUsuario = memoryData.GetData(Constante.CMIdUsuario);

            if (idTerminal > 0 && idUsuario > 0)
            {
                if (view == null && fragment != null) { view = fragment.View; }
                if (new Networks(context).VerificaConexion())
                {
                    var progressDialog = ProgressDialog.Show(context, context.GetString(Resource.String.progress_titulo), context.GetString(Resource.String.progress_mensaje), true);
                    try
                    {
                        Proxy.ProxyGestorRx gestorRx = new Proxy.ProxyGestorRx();
                        lista = await gestorRx.ObtenerListaInventario(idUsuario, idTerminal);

                        RecyclerView rvInventario = view.FindViewById<RecyclerView>(Resource.Id.recyclerViewInv);
                        RecyclerView.LayoutManager rvInventarioLayout = new LinearLayoutManager(context, LinearLayoutManager.Vertical, false);
                        rvInventario.SetLayoutManager(rvInventarioLayout);
                        RecyclerView.Adapter rvInventarioAdapter = new InventarioAdapter(context, lista, rvInventario);
                        rvInventario.SetAdapter(rvInventarioAdapter);
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