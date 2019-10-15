using Android.App;
using Android.Content;
using Android.Support.Design.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using CMSoftInventario.App.Model;
using CMSoftInventario.App.Util;
using System;

namespace CMSoftInventario.App.ViewModel
{
    public class LoginViewModels
    {
        private MemoryData memoryData;

        public LoginViewModels() { }

        public async void Acceder(Activity activity, EditText etUsuario, EditText etClave)
        {
            memoryData = MemoryData.GetInstance(activity);

            bool cancel = false;
            View focusView = null;
            string mensaje = string.Empty;
            int idTerminal = memoryData.GetData(Constante.CMIdterminal);

            Login login = new Login();
            login.Usuario = etUsuario.Text;
            login.Contrasena = etClave.Text;
            login.IdTerminal = idTerminal;

            if (TextUtils.IsEmpty(login.Usuario))
            {
                mensaje = string.Concat(mensaje, "- ", activity.GetString(Resource.String.login_val_usuario), "\n");
                etUsuario.SetError(activity.GetString(Resource.String.login_val_usuario), null);
                focusView = etUsuario;
                cancel = true;
            }

            if (TextUtils.IsEmpty(login.Contrasena))
            {
                focusView = string.IsNullOrEmpty(mensaje) ? etClave : etUsuario;
                mensaje = string.Concat(mensaje, "- ", activity.GetString(Resource.String.login_val_clave), "\n");
                etClave.SetError(activity.GetString(Resource.String.login_val_clave), null);
                cancel = true;
            }

            if (!string.IsNullOrEmpty(mensaje)) { Utilitarios.MensajeAdvertencia(activity, mensaje); }

            if (cancel) { focusView.RequestFocus(); }
            else
            {
                focusView = etClave;
                if (new Networks(activity).VerificaConexion())
                {
                    var progressDialog = ProgressDialog.Show(activity, activity.GetString(Resource.String.progress_titulo), activity.GetString(Resource.String.progress_mensaje_login), true);
                    try
                    {
                        Proxy.ProxyGestorRx gestorRx = new Proxy.ProxyGestorRx();
                        Usuario usuario = await gestorRx.ObtenerAcceso(login);
                        if (usuario.Respuesta.ToLower() == Constante.RespuestaCorrecto.ToLower())
                        {
                            memoryData.SaveData(Constante.CMIdUsuario, int.Parse(usuario.IdUsuario));
                            memoryData.SaveDataString(Constante.CMUsuario, login.Usuario);

                            activity.FinishAffinity();
                            activity.StartActivity(new Intent(activity, typeof(MainActivity)));
                            activity.OverridePendingTransition(Resource.Drawable.slide_in, Resource.Drawable.slide_out);
                        }
                        else { Snackbar.Make(focusView, Resource.String.login_invalido, Snackbar.LengthLong).Show(); }
                    }
                    catch (ApplicationException) { Snackbar.Make(focusView, Resource.String.app_error, Snackbar.LengthLong).Show(); }
                    catch (Exception) { Snackbar.Make(focusView, Resource.String.app_error, Snackbar.LengthLong).Show(); }
                    finally { progressDialog.Hide(); }
                }
                else { Snackbar.Make(focusView, Resource.String.app_network, Snackbar.LengthLong).Show(); }
            }

        }
    }
}