using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using CMSoftInventario.App;
using CMSoftInventario.App.Configuration;
using CMSoftInventario.App.Model;
using CMSoftInventario.App.Util;
using CMSoftInventario.App.ViewModel;
using System;

namespace CMSoftInventario
{
    [Activity(Label = "@string/app_name", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class LoginActivity : Activity
    {
        private MemoryData memoryData;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_login);

            Button btnIngresar = FindViewById<Button>(Resource.Id.sign_in_button);

            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.ReadPhoneState))
            {
                var requiredPermissions = new String[] { Manifest.Permission.ReadPhoneState };

                Snackbar.Make(btnIngresar, Resource.String.app_permiso_info, Snackbar.LengthIndefinite)
                        .SetAction(Resource.String.app_permiso_ok, new Action<View>(delegate (View obj) { ActivityCompat.RequestPermissions(this, requiredPermissions, 0); }
                        )).Show();
            }
            else { ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.ReadPhoneState }, 0); }

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadPhoneState) == (int)Permission.Granted)
            {
                Window window = Window;
                window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                window.SetStatusBarColor(Android.Graphics.Color.ParseColor(this.GetString(Resource.Color.colorPrimaryDark)));

                EditText etUsuario = FindViewById<EditText>(Resource.Id.usuario);
                EditText etClave = FindViewById<EditText>(Resource.Id.password);

                ConfigurationManager.Initialize(new AndroidConfigurationStreamProviderFactory(() => this));

                memoryData = MemoryData.GetInstance(this);
                int idTerminal = 0;
                var progressDialog = ProgressDialog.Show(this, this.GetString(Resource.String.progress_titulo), this.GetString(Resource.String.progress_mensaje), true);
                string id = Utilitarios.ObtenerID(this);
                try
                {
                    App.Proxy.ProxyGestorRx gestorRx = new App.Proxy.ProxyGestorRx();
                    Equipo equipo = await gestorRx.ObtenerEquipo(id);
                    Globals.Equipo = equipo;
                    idTerminal = equipo.IdEquipo;
                    memoryData.SaveData(Constante.CMIdterminal, equipo.IdEquipo);
                }
                catch (ApplicationException) { Snackbar.Make(btnIngresar, Resource.String.app_error, Snackbar.LengthLong).Show(); }
                catch (Exception) { Snackbar.Make(btnIngresar, Resource.String.app_error, Snackbar.LengthLong).Show(); }
                finally { progressDialog.Hide(); }
                //}
                if (idTerminal == 0) { Utilitarios.MensajeAdvertencia(this, string.Concat(this.GetString(Resource.String.app_equipo_noregistrado), ". (Serie : ", id, ")")); }
                if (memoryData.GetData(Constante.CMIdUsuario) == 0)
                {
                    btnIngresar.Click += delegate
                    {
                        if (idTerminal == 0) { Utilitarios.MensajeAdvertencia(this, string.Concat(this.GetString(Resource.String.app_equipo_noregistrado), ". (Serie : ", id, ")")); }
                        else
                        {
                            LoginViewModels vm = new LoginViewModels();
                            vm.Acceder(this, etUsuario, etClave);
                        }
                    };

                    etClave.EditorAction += (sender, e) =>
                    {
                        if (e.ActionId == ImeAction.Done)
                        {
                            if (idTerminal == 0) { Utilitarios.MensajeAdvertencia(this, string.Concat(this.GetString(Resource.String.app_equipo_noregistrado), ". (Serie : ", id, ")")); }
                            else
                            {
                                LoginViewModels vm = new LoginViewModels();
                                vm.Acceder(this, etUsuario, etClave);
                            }
                        }
                        else if (e.ActionId == ImeAction.Next)
                        {
                            if (idTerminal == 0) { Utilitarios.MensajeAdvertencia(this, string.Concat(this.GetString(Resource.String.app_equipo_noregistrado), " ", id)); }
                            else
                            {
                                LoginViewModels vm = new LoginViewModels();
                                vm.Acceder(this, etUsuario, etClave);
                            }
                        }
                        else { e.Handled = false; }
                    };
                }
                else { StartActivity(new Intent(this, typeof(MainActivity)).SetFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask)); }
            }
            else
            {
                Snackbar.Make(btnIngresar, Resource.String.app_error_permiso, Snackbar.LengthLong).Show();
                Process.KillProcess(Process.MyPid());
            }
        }
    }
}