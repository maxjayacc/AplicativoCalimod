using Android.App;
using Android.OS;
using CMSoftInventario.App.Util;
using CMSoftInventario.App.ViewModel;

namespace CMSoftInventario.App.Fragment
{
    public class KitDialogFragment : DialogFragment
    {
        private Activity activity;
        private IConsultorArgument consultor;
        private LecturaKitViewModel vm;

        public KitDialogFragment(Activity activity, IConsultorArgument consultor)
        {
            this.activity = activity;
            this.consultor = consultor;
        }

        public static KitDialogFragment NewInstance(Activity activity, IConsultorArgument consultor)
        {

            var dialogFragment = new KitDialogFragment(activity, consultor);
            return dialogFragment;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var builder = new AlertDialog.Builder(Activity);
            var inflater = Activity.LayoutInflater;
            var dialogView = inflater.Inflate(Resource.Layout.lectura_kit, null);
            builder.SetView(dialogView);
            var dialog = builder.Create();
            if (dialogView != null) { vm = new LecturaKitViewModel(activity, dialogView, dialog); }

            return dialog;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (vm != null) { vm.DestroyScanner(); }
            if (consultor != null) { consultor.devuelveObjeto(null, Enumerador.eConsulta.Kit); }
        }
    }
}