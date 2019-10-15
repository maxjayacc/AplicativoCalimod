using Android.App;
using Android.OS;
using CMSoftInventario.App.Util;
using CMSoftInventario.App.ViewModel;

namespace CMSoftInventario.App.Fragment
{
    public class ItemDialogFragment : DialogFragment
    {
        private Activity activity;
        private IConsultorArgument consultor;
        private LecturaItemViewModel vm;
        public ItemDialogFragment(Activity activity, IConsultorArgument consultor)
        {
            this.activity = activity;
            this.consultor = consultor;
        }

        public static ItemDialogFragment NewInstance(Activity activity, IConsultorArgument consultor)
        {

            var dialogFragment = new ItemDialogFragment(activity, consultor);
            return dialogFragment;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var builder = new AlertDialog.Builder(Activity);
            var inflater = Activity.LayoutInflater;
            var dialogView = inflater.Inflate(Resource.Layout.lectura_item, null);

            builder.SetView(dialogView);
            var dialog = builder.Create();
            if (dialogView != null) { vm = new LecturaItemViewModel(activity, dialogView, dialog); }

            return dialog;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (vm != null) { vm.DestroyScanner(); }
            if (consultor != null) { consultor.devuelveObjeto(null, Enumerador.eConsulta.Item); }
        }
    }
}