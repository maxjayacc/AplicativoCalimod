using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using CMSoftInventario.App;
using CMSoftInventario.App.Adapter;
using CMSoftInventario.App.Fragment;
using CMSoftInventario.App.Model;
using CMSoftInventario.App.Util;
using CMSoftInventario.App.ViewModel;

namespace CMSoftInventario
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {

        private MemoryData memoryData;
        private TabLayout tabLayout;
        private ViewPager viewPager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Window.AddFlags(WindowManagerFlags.KeepScreenOn | WindowManagerFlags.DismissKeyguard | WindowManagerFlags.ShowWhenLocked | WindowManagerFlags.TurnScreenOn);

            var fragments = new Android.Support.V4.App.Fragment[]
            {
                new InventarioFragment(),
                new UbicacionFragment(),
                new ItemFragment(),
            };

            var titles = CharSequence.ArrayFromStringArray(new[] {
                GetString(Resource.String.tab_inventario),
                GetString(Resource.String.tab_ubicacion),
                GetString(Resource.String.tab_item),
            });

            viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            viewPager.Adapter = new TabsFragmentPagerAdapter(SupportFragmentManager, fragments, titles);
            viewPager.Adapter.NotifyDataSetChanged();
            viewPager.OffscreenPageLimit = titles.Length;

            viewPager.PageSelected += (object sender, ViewPager.PageSelectedEventArgs e) =>
            {
                Parametro parametro = Globals.Parametro;
                if (e.Position == 0)
                {
                    Android.Support.V4.App.Fragment fragment = ((TabsFragmentPagerAdapter)viewPager.Adapter).GetFragment(e.Position);
                    if (fragment != null)
                    {
                        parametro.IdAsignacionUbicacion = 0;
                        parametro.IdUbicacion = 0;
                        parametro.NombreUbicacion = string.Empty;
                        parametro.EstadoUbicacion = string.Empty;
                        parametro.CantidadReconteo = 0;
                        parametro.Reconteo = string.Empty;
                        parametro.DesReconteo = string.Empty;
                        parametro.SelectedPosition = -1;

                        InventarioViewModel vm = new InventarioViewModel();
                        vm.CargarListado(this, null, fragment);
                    }
                }
                else if (e.Position == 1)
                {
                    Android.Support.V4.App.Fragment fragment = ((TabsFragmentPagerAdapter)viewPager.Adapter).GetFragment(e.Position);
                    if (fragment != null)
                    {
                        parametro.Reconteo = string.Empty;
                        parametro.DesReconteo = string.Empty;

                        UbicacionViewModel vm = new UbicacionViewModel();
                        vm.CargarListado(this, fragment);
                    }
                }
                else if (e.Position == 2)
                {
                    Android.Support.V4.App.Fragment fragment = ((TabsFragmentPagerAdapter)viewPager.Adapter).GetFragment(e.Position);
                    if (fragment != null)
                    {
                        ItemReconteoViewModel vm = new ItemReconteoViewModel();
                        vm.CargarListado(this, fragment);
                    }
                }
                Globals.Parametro = parametro;
            };

            tabLayout = FindViewById<TabLayout>(Resource.Id.sliding_tabs);
            tabLayout.SetupWithViewPager(viewPager);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Drawable.actionbar_main, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.info_equipo)
            {
                var dialog = EquipoDialogFragment.NewInstance(this);
                dialog.Show(this.FragmentManager, "Info Equipo");
            }
            else if (item.ItemId == Resource.Id.sigout)
            {
                memoryData = MemoryData.GetInstance(this);
                memoryData.SaveData(Constante.CMIdUsuario, 0);

                memoryData.SaveData(Constante.CMIdterminal, 0);

                FinishAffinity();
                StartActivity(new Intent(this, typeof(LoginActivity)).SetFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask));
            }

            return base.OnOptionsItemSelected(item);
        }


    }

}