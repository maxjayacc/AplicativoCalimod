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
    public class ItemReconteoViewModel
    {
        private MemoryData memoryData;
        public async void CargarListado(Activity context, Android.Support.V4.App.Fragment fragment)
        {
            memoryData = MemoryData.GetInstance(context);
            List<ItemReconteo> lista = new List<ItemReconteo>();
            int idTerminal = memoryData.GetData(Constante.CMIdterminal);
            int idUsuario = memoryData.GetData(Constante.CMIdUsuario);
            Parametro parametro = Globals.Parametro;

            View view = fragment.View;
            TextView tvCompania = view.FindViewById<TextView>(Resource.Id.ritem_compania);
            TextView tvInventario = view.FindViewById<TextView>(Resource.Id.ritem_inventario);
            TextView tvAlmacen = view.FindViewById<TextView>(Resource.Id.ritem_almacen);
            TextView tvUbicacion = view.FindViewById<TextView>(Resource.Id.ritem_ubicacion);

            RecyclerView rvItem = view.FindViewById<RecyclerView>(Resource.Id.recyclerViewItem);
            RecyclerView.LayoutManager rvItemLayout = new LinearLayoutManager(context, LinearLayoutManager.Vertical, false);
            RecyclerView.Adapter rvItemAdapter;

            if (idUsuario > 0 && idTerminal > 0 && parametro.IdAsignacionUbicacion > 0 && parametro.CantidadReconteo > 1)
            {

                if (new Networks(context).VerificaConexion())
                {
                    var progressDialog = ProgressDialog.Show(context, context.GetString(Resource.String.progress_titulo), context.GetString(Resource.String.progress_mensaje), true);
                    try
                    {
                        Proxy.ProxyGestorRx gestorRx = new Proxy.ProxyGestorRx();
                        lista = await gestorRx.ObtenerListaItemReconteo(parametro.IdAsignacionUbicacion);

                        tvCompania.Text = parametro.CompaniaDescripcion;
                        tvInventario.Text = parametro.NombreInventario;
                        tvAlmacen.Text = parametro.AlmacenDescripcion;
                        tvUbicacion.Text = parametro.NombreUbicacion;

                        rvItem.SetLayoutManager(rvItemLayout);
                        rvItemAdapter = new ItemReconteoAdapter(context, lista, rvItem);
                        rvItem.SetAdapter(rvItemAdapter);
                    }
                    catch (ApplicationException) { Snackbar.Make(view, Resource.String.app_error, Snackbar.LengthLong).Show(); }
                    catch (Exception) { Snackbar.Make(view, Resource.String.app_error, Snackbar.LengthLong).Show(); }
                    finally { progressDialog.Hide(); }
                }
                else { Snackbar.Make(view, Resource.String.app_network, Snackbar.LengthLong).Show(); }
            }
            else
            {
                tvCompania.Text = string.Empty;
                tvInventario.Text = string.Empty;
                tvAlmacen.Text = string.Empty;
                tvUbicacion.Text = string.Empty;

                rvItem.SetLayoutManager(rvItemLayout);
                rvItemAdapter = new ItemReconteoAdapter(context, new List<ItemReconteo>(), rvItem);
                rvItem.SetAdapter(rvItemAdapter);
            }
        }
    }
}