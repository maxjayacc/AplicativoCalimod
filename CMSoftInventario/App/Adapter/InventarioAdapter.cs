using Android.App;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using CMSoftInventario.App.Model;
using System;
using System.Collections.Generic;

namespace CMSoftInventario.App.Adapter
{
    public class InventarioAdapter : RecyclerView.Adapter
    {
        private RecyclerView rvInventario;
        private List<Inventario> lista;
        private Activity activity;

        public InventarioAdapter(Activity activity, List<Inventario> lista, RecyclerView rvInventario)
        {
            this.activity = activity;
            this.lista = lista;
            this.rvInventario = rvInventario;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View listitem = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_row_inv, parent, false);
            TextView tvNombre = listitem.FindViewById<TextView>(Resource.Id.row_inv_nombre);
            TextView tvCompania = listitem.FindViewById<TextView>(Resource.Id.row_inv_compania);
            TextView tvAlmacen = listitem.FindViewById<TextView>(Resource.Id.row_inv_almacen);
            TextView tvEstado = listitem.FindViewById<TextView>(Resource.Id.row_inv_estado);

            InventarioAdapterViewHolder view = new InventarioAdapterViewHolder(listitem);
            view.tvNombre = tvNombre;
            view.tvCompania = tvCompania;
            view.tvAlmacen = tvAlmacen;
            view.tvEstado = tvEstado;
            return view;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            InventarioAdapterViewHolder holder = viewHolder as InventarioAdapterViewHolder;
            holder.mainview.Click += mMainView_Click;
            var item = lista[position];
            holder.tvNombre.Text = item.NombreInventario;
            holder.tvCompania.Text = item.CompaniaDescripcion;
            holder.tvAlmacen.Text = item.AlmacenDescripcion;
            holder.tvEstado.Text = item.EstadoDescripcion;
        }

        private void mMainView_Click(object sender, EventArgs e)
        {
            int position = rvInventario.GetChildAdapterPosition((View)sender);
            int indexPosition = (lista.Count - 1) - position;

            Parametro parametro = new Parametro();
            parametro.IdInventario = lista[position].IdInventario;
            parametro.IdTipo = lista[position].IdTipo;
            parametro.CompaniaDescripcion = lista[position].CompaniaDescripcion;
            parametro.NombreInventario = lista[position].NombreInventario;
            parametro.AlmacenDescripcion = lista[position].AlmacenDescripcion;
            Globals.Parametro = parametro;

            TabLayout tabLayout = activity.FindViewById<TabLayout>(Resource.Id.sliding_tabs);
            ViewPager viewPager = activity.FindViewById<ViewPager>(Resource.Id.viewpager);
            tabLayout.SetScrollPosition(1, 0f, true);
            viewPager.SetCurrentItem(1, false);

        }

        public override int ItemCount
        {
            get { return lista.Count; }
        }

    }

    public class InventarioAdapterViewHolder : RecyclerView.ViewHolder
    {
        public View mainview { get; set; }
        public TextView tvNombre { get; set; }
        public TextView tvCompania { get; set; }
        public TextView tvAlmacen { get; set; }
        public TextView tvEstado { get; set; }

        public InventarioAdapterViewHolder(View view) : base(view)
        {
            mainview = view;
        }
    }
}