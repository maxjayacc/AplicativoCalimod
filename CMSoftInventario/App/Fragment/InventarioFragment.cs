using Android.App;
using Android.OS;
using Android.Views;

using CMSoftInventario.App.ViewModel;

namespace CMSoftInventario.App.Fragment
{
    public class InventarioFragment : Android.Support.V4.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.inventario, container, false);
            InventarioViewModel vm = new InventarioViewModel();
            vm.CargarListado((Activity)container.Context, view);
            return view;

        }


    }
}