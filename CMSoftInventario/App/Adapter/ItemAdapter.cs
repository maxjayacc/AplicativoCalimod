using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using CMSoftInventario.App.Model;
using System.Collections.Generic;

namespace CMSoftInventario.App.Adapter
{
    public class ItemAdapter : RecyclerView.Adapter
    {
        private RecyclerView rvInventario;
        private List<KitItem> lista;
        private Activity activity;

        public ItemAdapter(Activity activity, List<KitItem> lista, RecyclerView rvInventario)
        {
            this.activity = activity;
            this.lista = lista;
            this.rvInventario = rvInventario;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View listitem = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.lectura_kit_item_row, parent, false);
            TextView tvItem = listitem.FindViewById<TextView>(Resource.Id.row_kit_item_item);
            TextView tvCantidad = listitem.FindViewById<TextView>(Resource.Id.row_kit_item_cantidad);

            ItemAdapterViewHolder view = new ItemAdapterViewHolder(listitem);
            view.tvItem = tvItem;
            view.tvCantidad = tvCantidad;
            return view;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            ItemAdapterViewHolder holder = viewHolder as ItemAdapterViewHolder;
            var item = lista[position];
            holder.tvItem.Text = item.Codigo_Subordinado;
            holder.tvCantidad.Text = item.Cantidad_Tomada.ToString();
        }

        public override int ItemCount
        {
            get { return lista.Count; }
        }
    }

    public class ItemAdapterViewHolder : RecyclerView.ViewHolder
    {
        public View mainview { get; set; }
        public TextView tvItem { get; set; }
        public TextView tvCantidad { get; set; }

        public ItemAdapterViewHolder(View view) : base(view)
        {
            mainview = view;
        }
    }
}