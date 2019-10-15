using Android.OS;
using Android.Views;

namespace CMSoftInventario.App.Fragment
{
    public class UbicacionFragment : Android.Support.V4.App.Fragment
    {
        View view;
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
            view = inflater.Inflate(Resource.Layout.ubicacion, container, false);
            return view;
        }

        //public override void OnResume()
        //{
        //    base.OnResume();
        //    //View view = LayoutInflater.From(viewPager.Context).Inflate(Resource.Layout.ubicacion, viewPager, false);
        //    UbicacionViewModel vm = new UbicacionViewModel();
        //    vm.CargarListado((Activity)view.Context, view);
        //}
    }
}