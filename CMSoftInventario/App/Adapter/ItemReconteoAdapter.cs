using Android.App;
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
    public class ItemReconteoAdapter : RecyclerView.Adapter, IConsultorArgument
    {
        private RecyclerView rvItem;
        private MemoryData memoryData;
        private List<ItemReconteo> lista;
        private Activity activity;
        private bool isOpen = false;
        //private static SparseBooleanArray selectedItems = new SparseBooleanArray();
        int selectedPosition = -1;

        public ItemReconteoAdapter(Activity activity, List<ItemReconteo> lista, RecyclerView rvItem)
        {
            this.activity = activity;
            this.lista = lista;
            this.rvItem = rvItem;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View listitem = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_row_item, parent, false);
            TextView tvItem = listitem.FindViewById<TextView>(Resource.Id.row_item_item);
            TextView tvDescripcion = listitem.FindViewById<TextView>(Resource.Id.row_item_descripcion);
            ImageView ivCerrar = listitem.FindViewById<ImageView>(Resource.Id.row_item_cerrar);

            ItemReconteoAdapterViewHolder view = new ItemReconteoAdapterViewHolder(listitem);
            view.tvItem = tvItem;
            view.tvDescripcion = tvDescripcion;
            view.ivCerrar = ivCerrar;
            return view;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            ItemReconteoAdapterViewHolder holder = viewHolder as ItemReconteoAdapterViewHolder;
            //holder.mainview.Selected = selectedItems.Get(position, false);
            if (selectedPosition == position) { holder.ItemView.SetBackgroundResource(Resource.Color.colorGray); }
            else { holder.ItemView.SetBackgroundResource(Resource.Color.colorText); ; }

            holder.mainview.Click += mMainView_Click;
            var item = lista[position];
            holder.tvItem.Text = item.Item;
            holder.tvDescripcion.Text = item.Descripcion;
            holder.ivCerrar.Click += ivCerrar_Click;
        }

        private async void ivCerrar_Click(object sender, EventArgs e)
        {
            if (new Networks(activity).VerificaConexion())
            {
                var progressDialog = ProgressDialog.Show(activity, activity.GetString(Resource.String.progress_titulo), activity.GetString(Resource.String.progress_mensaje_cerrando), true);
                try
                {
                    memoryData = MemoryData.GetInstance(activity);
                    Parametro parametro = Globals.Parametro;

                    int idTerminal = memoryData.GetData(Constante.CMIdterminal);
                    int idUsuario = memoryData.GetData(Constante.CMIdUsuario);

                    int position = rvItem.GetChildAdapterPosition((View)((ImageView)sender).Parent);
                    ItemCerrar itemCerrar = new ItemCerrar();
                    itemCerrar.idUsuario = idUsuario;
                    itemCerrar.idInventario = parametro.IdInventario;
                    itemCerrar.idUbicacion = parametro.IdUbicacion;
                    itemCerrar.idAsignacionUbicacion = parametro.IdAsignacionUbicacion;
                    itemCerrar.idItem = lista[position].IdItem;

                    Proxy.ProxyGestorTx gestorTx = new Proxy.ProxyGestorTx();
                    string resul = await gestorTx.CerrarUbicacionItemLocal(itemCerrar);
                    if (resul.ToLower() == Constante.RespuestaExito.ToLower())
                    {
                        parametro.Reconteo = string.Empty;
                        parametro.DesReconteo = string.Empty;

                        //selectedItems = new SparseBooleanArray();
                        selectedPosition = -1;
                        await Refrescar(idUsuario, idTerminal, parametro.IdAsignacionUbicacion, parametro.CantidadReconteo);
                        Toast.MakeText(activity, activity.GetString(Resource.String.itemreconteo_cerrar_correcto), ToastLength.Short).Show();
                        //TabLayout tabLayout = activity.FindViewById<TabLayout>(Resource.Id.sliding_tabs);
                        //ViewPager viewPager = activity.FindViewById<ViewPager>(Resource.Id.viewpager);
                        //tabLayout.SetScrollPosition(2, 0f, true);
                        //viewPager.SetCurrentItem(2, false);

                        ////Modificación
                        //((ImageView)sender).Visibility = ViewStates.Gone;
                        //NotifyItemRemoved(position);
                        //lista.RemoveAt(position);
                        //NotifyDataSetChanged();
                        ////

                    }
                    else { Toast.MakeText(activity, activity.GetString(Resource.String.itemreconteo_cerrar_incorrecto), ToastLength.Short).Show(); }
                }
                catch (ApplicationException) { Toast.MakeText(activity, activity.GetString(Resource.String.app_error), ToastLength.Long).Show(); }
                catch (Exception) { Toast.MakeText(activity, activity.GetString(Resource.String.app_error), ToastLength.Long).Show(); }
                finally { progressDialog.Hide(); }
            }
            else { Toast.MakeText(activity, activity.GetString(Resource.String.app_network), ToastLength.Long).Show(); }
        }


        private async Task Refrescar(int idUsuario, int idTerminal, int idAsignacionUbicacion, int reconteo)
        {
            RecyclerView.LayoutManager rvItemLayout = new LinearLayoutManager(activity, LinearLayoutManager.Vertical, false);
            RecyclerView.Adapter rvItemAdapter;

            if (idUsuario > 0 && idTerminal > 0 && idAsignacionUbicacion > 0 && reconteo > 1)
            {

                if (new Networks(activity).VerificaConexion())
                {
                    var progressDialog = ProgressDialog.Show(activity, activity.GetString(Resource.String.progress_titulo), activity.GetString(Resource.String.progress_mensaje), true);
                    try
                    {
                        Proxy.ProxyGestorRx gestorRx = new Proxy.ProxyGestorRx();
                        lista = await gestorRx.ObtenerListaItemReconteo(idAsignacionUbicacion);

                        rvItem.SetLayoutManager(rvItemLayout);
                        rvItemAdapter = new ItemReconteoAdapter(activity, lista, rvItem);
                        rvItem.SetAdapter(rvItemAdapter);
                    }
                    catch (ApplicationException) { Toast.MakeText(activity, activity.GetString(Resource.String.app_error), ToastLength.Long).Show(); }
                    catch (Exception) { Toast.MakeText(activity, activity.GetString(Resource.String.app_error), ToastLength.Long).Show(); }
                    finally { progressDialog.Hide(); }
                }
                else { Toast.MakeText(activity, activity.GetString(Resource.String.app_network), ToastLength.Long).Show(); }
            }
            else
            {
                rvItem.SetLayoutManager(rvItemLayout);
                rvItemAdapter = new ItemReconteoAdapter(activity, new List<ItemReconteo>(), rvItem);
                rvItem.SetAdapter(rvItemAdapter);
            }
        }

        private void mMainView_Click(object sender, EventArgs e)
        {
            int position = rvItem.GetChildAdapterPosition((View)sender);
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
                parametro.Reconteo = lista[position].Item;
                parametro.DesReconteo = lista[position].Descripcion;

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

    public class ItemReconteoAdapterViewHolder : RecyclerView.ViewHolder
    {
        public View mainview { get; set; }
        public TextView tvItem { get; set; }
        public TextView tvDescripcion { get; set; }
        public ImageView ivCerrar { get; set; }

        public ItemReconteoAdapterViewHolder(View view) : base(view)
        {
            mainview = view;
        }
    }

}