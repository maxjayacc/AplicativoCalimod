using Android.App;
using Android.OS;
using Android.Widget;


using CMSoftInventario.App.Util;

namespace CMSoftInventario.App.Fragment
{
    public class EquipoDialogFragment : DialogFragment
    {
        private Activity activity;

        public EquipoDialogFragment(Activity activity)
        {
            this.activity = activity;
        }

        public static EquipoDialogFragment NewInstance(Activity activity)
        {

            var dialogFragment = new EquipoDialogFragment(activity);
            return dialogFragment;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var builder = new AlertDialog.Builder(Activity);
            var inflater = Activity.LayoutInflater;
            var dialogView = inflater.Inflate(Resource.Layout.equipo, null);

            MemoryData memoryData = MemoryData.GetInstance(activity);

            TextView tvUsuario = dialogView.FindViewById<TextView>(Resource.Id.eq_usuario);
            TextView tvModelo = dialogView.FindViewById<TextView>(Resource.Id.eq_modelo);
            TextView tvFabricante = dialogView.FindViewById<TextView>(Resource.Id.eq_fabricante);
            TextView tvNombre = dialogView.FindViewById<TextView>(Resource.Id.eq_nombre);
            TextView tvProducto = dialogView.FindViewById<TextView>(Resource.Id.eq_producto);
            TextView tvSerie = dialogView.FindViewById<TextView>(Resource.Id.eq_serie);
            TextView tvMarca = dialogView.FindViewById<TextView>(Resource.Id.eq_marca);
            TextView tvNumero = dialogView.FindViewById<TextView>(Resource.Id.eq_numero);
            Button btnOk = dialogView.FindViewById<Button>(Resource.Id.eq_info_ok);

            tvUsuario.Text = memoryData.GetDataString(Constante.CMUsuario);
            tvModelo.Text = Build.Model;
            tvMarca.Text = Build.Brand;
            tvFabricante.Text = Build.Manufacturer;
            tvNombre.Text = Build.Host.ToUpper();
            tvProducto.Text = Build.Product;
            tvSerie.Text = Build.Serial;
            tvNumero.Text = Utilitarios.ObtenerID(activity);

            builder.SetView(dialogView);
            var dialog = builder.Create();

            btnOk.Click += (sender, e) => { dialog.Dismiss(); };
            return dialog;
        }
    }
}