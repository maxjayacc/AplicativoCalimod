using Android.App;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using CMSoftInventario.App.Fragment;
using CMSoftInventario.App.Model;
using CMSoftInventario.App.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMSoftInventario.App.Adapter
{
    public class UbicacionAdapter : RecyclerView.Adapter, IConsultorArgument
    {
        private RecyclerView rvUbicacion;
        List<Ubicacion> lista;
        private Activity activity;
        private MemoryData memoryData;
        private bool isOpen = false;

        //private static SparseBooleanArray selectedItems = new SparseBooleanArray();
        int selectedPosition = -1;

        public UbicacionAdapter(Activity activity, List<Ubicacion> lista, RecyclerView rvUbicacion)
        {
            this.activity = activity;
            this.lista = lista;
            this.rvUbicacion = rvUbicacion;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View listitem = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_row_ubi, parent, false);
            TextView tvNombre = listitem.FindViewById<TextView>(Resource.Id.row_ubi_nombre);
            TextView tvEstado = listitem.FindViewById<TextView>(Resource.Id.row_ubi_estado);
            ImageView ivCerrar = listitem.FindViewById<ImageView>(Resource.Id.row_ubi_cerrar);

            UbicacionAdapterViewHolder view = new UbicacionAdapterViewHolder(listitem);
            view.tvNombre = tvNombre;
            view.tvEstado = tvEstado;
            view.ivCerrar = ivCerrar;
            return view;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            UbicacionAdapterViewHolder holder = viewHolder as UbicacionAdapterViewHolder;
            //holder.mainview.Selected = selectedItems.Get(position, false);
            Parametro parametro = Globals.Parametro;
            //if (parametro.SelectedPosition > -1)
            //{
            //    if (parametro.SelectedPosition == position){ holder.ItemView.SetBackgroundResource(Resource.Color.colorGray); }
            //}
            //else
            //{
            if (selectedPosition == position) { holder.ItemView.SetBackgroundResource(Resource.Color.colorGray); }
            else { holder.ItemView.SetBackgroundResource(Resource.Color.colorText); ; }
            //}

            holder.mainview.Click += mMainView_Click;
            var item = lista[position];
            holder.tvNombre.Text = item.NombreUbicacion;
            holder.tvEstado.Text = item.EstadoDescripcion;
            holder.ivCerrar.Click += ivCerrar_Click;
        }


        public int i = 0;
        public int y = 0;

        private async void ivCerrar_Click(object sender, EventArgs e)
        {

            bool correct = false;


            if (i == 0)
            {

                AlertDialog.Builder alert = new AlertDialog.Builder(activity);
                alert.SetTitle("Ubicacion");
                alert.SetMessage("Cerrar ubicación.");
                alert.SetPositiveButton("Confirmar", (senderAlert, args) =>
                {
                    Toast.MakeText(activity, "Ud. confirmo El cierre", ToastLength.Short).Show();
                    i = 1;

                    ((ImageView)sender).Visibility = ViewStates.Gone;
                    ivCerrar_Click(sender, e);
                    alert.Dispose();

                });

                alert.SetNegativeButton("Cancelar", (senderAlert, args) =>
                {
                    Toast.MakeText(activity, "Cancelado !", ToastLength.Short).Show();
                    alert.Dispose();

                });
                Dialog dialog = alert.Create();
                dialog.Show();
            }


            else if (new Networks(activity).VerificaConexion())
            {

                var progressDialog = ProgressDialog.Show(activity, activity.GetString(Resource.String.progress_titulo), activity.GetString(Resource.String.progress_mensaje_cerrando), true);

                try
                {
                    memoryData = MemoryData.GetInstance(activity);
                    Parametro parametro = Globals.Parametro;

                    int idTerminal = memoryData.GetData(Constante.CMIdterminal);
                    int idUsuario = memoryData.GetData(Constante.CMIdUsuario);


                    int position = rvUbicacion.GetChildAdapterPosition((View)((ImageView)sender).Parent);
                    UbicacionCerrar ubiCerrar = new UbicacionCerrar();
                    ubiCerrar.idUsuario = idUsuario;
                    ubiCerrar.idInventario = parametro.IdInventario;
                    ubiCerrar.idUbicacion = lista[position].IdUbicacion;
                    ubiCerrar.idAsignacionUbicacion = lista[position].IdAsignacionUbicacion;
                    ubiCerrar.EsInventarioKit = parametro.IdTipo == Constante.IdTipoKit ? 1 : 0;

                    UbicacionCerrarMiddleware ubiCerrarMid = new UbicacionCerrarMiddleware();
                    ubiCerrarMid.idUsuario = idUsuario;
                    ubiCerrarMid.idInventario = parametro.IdInventario;
                    ubiCerrarMid.idUbicacion = lista[position].IdUbicacion;
                    ubiCerrarMid.idAsignacionUbicacion = lista[position].IdAsignacionUbicacion;
                    ubiCerrarMid.EsInventarioKit = parametro.IdTipo == Constante.IdTipoKit ? 1 : 0;

                    Proxy.ProxyGestorTx gestorTx = new Proxy.ProxyGestorTx();


                    string resul = await gestorTx.CerrarUbicacionLocal(ubiCerrar);

                    if (resul.ToLower() == Constante.RespuestaExito.ToLower())
                    {
                        correct = true;
                        Toast.MakeText(activity, activity.GetString(Resource.String.ubicacion_cerrar_correcto), ToastLength.Short).Show();

                        resul = await gestorTx.MiddlewareCerrarInterface(ubiCerrarMid);
                        if (resul.ToLower() == Constante.RespuestaExito.ToLower())
                        {
                            correct = true;
                            parametro.IdAsignacionUbicacion = 0;
                            parametro.IdUbicacion = 0;
                            parametro.NombreUbicacion = string.Empty;
                            parametro.EstadoUbicacion = string.Empty;
                            parametro.CantidadReconteo = 0;
                            parametro.Reconteo = string.Empty;
                            parametro.DesReconteo = string.Empty;
                            parametro.SelectedPosition = -1;

                            //selectedItems = new SparseBooleanArray();
                            selectedPosition = -1;
                            await Refrescar(idUsuario, idTerminal, parametro.IdInventario);
                            Toast.MakeText(activity, activity.GetString(Resource.String.ubicacion_cerrar_middle_correcto), ToastLength.Short).Show();
                        }
                        else { Toast.MakeText(activity, activity.GetString(Resource.String.ubicacion_cerrar_middle_incorrecto), ToastLength.Short).Show(); }
                    }

                    else { Toast.MakeText(activity, activity.GetString(Resource.String.ubicacion_cerrar_incorrecto), ToastLength.Long).Show(); }
                }
                catch (ApplicationException) { Toast.MakeText(activity, activity.GetString(Resource.String.app_error), ToastLength.Long).Show(); }
                catch (Exception) { Toast.MakeText(activity, activity.GetString(Resource.String.app_error), ToastLength.Long).Show(); }
                finally { progressDialog.Hide(); }

                if (!correct) { ((ImageView)sender).Visibility = ViewStates.Visible; }
            }
            else { Toast.MakeText(activity, activity.GetString(Resource.String.app_network), ToastLength.Long).Show(); }




        }

        private async Task Refrescar(int idUsuario, int idTerminal, int idInventario)
        {
            if (new Networks(activity).VerificaConexion())
            {
                var progressDialog = ProgressDialog.Show(activity, activity.GetString(Resource.String.progress_titulo), activity.GetString(Resource.String.progress_mensaje), true);
                try
                {
                    Proxy.ProxyGestorRx gestorRx = new Proxy.ProxyGestorRx();
                    lista = await gestorRx.ObtenerListaUbicacion(idUsuario, idTerminal, idInventario);

                    RecyclerView.LayoutManager rvUbicacionLayout = new LinearLayoutManager(activity, LinearLayoutManager.Vertical, false);
                    rvUbicacion.SetLayoutManager(rvUbicacionLayout);
                    RecyclerView.Adapter rvUbicacionAdapter = new UbicacionAdapter(activity, lista, rvUbicacion);
                    rvUbicacion.SetAdapter(rvUbicacionAdapter);
                }
                catch (ApplicationException) { Toast.MakeText(activity, activity.GetString(Resource.String.app_error), ToastLength.Long).Show(); }
                catch (Exception) { Toast.MakeText(activity, activity.GetString(Resource.String.app_error), ToastLength.Long).Show(); }
                finally { progressDialog.Hide(); }
            }
            else { Toast.MakeText(activity, activity.GetString(Resource.String.app_network), ToastLength.Long).Show(); }
        }

        private void mMainView_Click(object sender, EventArgs e)
        {
            int position = rvUbicacion.GetChildAdapterPosition((View)sender);
            int indexPosition = (lista.Count - 1) - position;
            if (position > -1)
            {

                //if (selectedItems.Get(position, false))
                //{
                //    selectedItems.Delete(position);
                //    ((View)sender).Selected = false;
                //}
                //else
                //{
                //    selectedItems.Put(position, true);
                //    ((View)sender).Selected = true;
                //}

                Parametro parametro = Globals.Parametro;
                parametro.IdUbicacion = lista[position].IdUbicacion;
                parametro.IdAsignacionUbicacion = lista[position].IdAsignacionUbicacion;
                parametro.NombreUbicacion = lista[position].NombreUbicacion;
                parametro.EstadoUbicacion = lista[position].EstadoCodigo;
                parametro.CantidadReconteo = lista[position].Reconteo;

                if (lista[position].Reconteo > 1)
                {
                    parametro.SelectedPosition = position;
                    Globals.Parametro = parametro;

                    TabLayout tabLayout = activity.FindViewById<TabLayout>(Resource.Id.sliding_tabs);
                    ViewPager viewPager = activity.FindViewById<ViewPager>(Resource.Id.viewpager);
                    tabLayout.SetScrollPosition(2, 0f, true);
                    viewPager.SetCurrentItem(2, false);
                }
                else
                {
                    Globals.Parametro = parametro;
                    if (!isOpen)
                    {
                        isOpen = true;
                        if (parametro.IdTipo == Constante.IdTipoKit)
                        {
                            var dialog = KitDialogFragment.NewInstance(activity, this);
                            dialog.Show(activity.FragmentManager, "Lectura Kit");
                        }
                        else
                        {
                            var dialog = ItemDialogFragment.NewInstance(activity, this);
                            dialog.Show(activity.FragmentManager, "Lectura Item");
                        }
                    }

                }
                selectedPosition = position;
                NotifyDataSetChanged();
            }
        }

        public override int ItemCount
        {
            get { return lista.Count; }
        }


        #region "   Funciones Publicas  "

        /// <summary>
        /// Obtiene valores de otras ventanas
        /// </summary>
        /// <param name="objeto"></param>
        /// <param name="tipoConsulta"></param>
        public void devuelveObjeto(object objeto, Enumerador.eConsulta tipoConsulta)
        {

            switch (tipoConsulta)
            {
                case Enumerador.eConsulta.Item:
                    isOpen = false;
                    break;
                case Enumerador.eConsulta.Kit:
                    isOpen = false;
                    break;
                default:
                    break;
            }

        }

        #endregion
    }

    public class UbicacionAdapterViewHolder : RecyclerView.ViewHolder
    {
        public View mainview { get; set; }
        public TextView tvNombre { get; set; }
        public TextView tvEstado { get; set; }
        public ImageView ivCerrar { get; set; }

        public UbicacionAdapterViewHolder(View view) : base(view)
        {
            mainview = view;
        }
    }
}