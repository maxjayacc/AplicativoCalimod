using Android.App;
using Android.OS;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Threading;

namespace CMSoftInventario.App.Util
{
    public static class Utilitarios
    {

        public static T DeserializarRest<T>(string rest)
        {
            T resul;

            if (typeof(T) == typeof(long) || typeof(T) == typeof(int) || typeof(T) == typeof(short) || typeof(T) == typeof(byte) ||
                typeof(T) == typeof(decimal) || typeof(T) == typeof(bool) || typeof(T) == typeof(byte[]) || typeof(T) == typeof(DateTime)) { resul = (T)Convert.ChangeType(rest, typeof(T)); }
            else
            {
                var settDate = new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat, DateTimeZoneHandling = DateTimeZoneHandling.Local };
                resul = JsonConvert.DeserializeObject<T>(rest, settDate);
            }
            return resul;
        }

        public static string SerializarRest<T>(T objeto)
        {
            string resul;
            var settDate = new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat, DateTimeZoneHandling = DateTimeZoneHandling.Local };
            resul = JsonConvert.SerializeObject(objeto, settDate);
            return resul;
        }

        public static string ObtenerID(Activity activity)
        {
            //var telephonyDeviceID = string.Empty;
            //var telephonySIMSerialNumber = string.Empty;
            //TelephonyManager telephonyManager = (TelephonyManager)activity.ApplicationContext.GetSystemService(Context.TelephonyService);
            //if (telephonyManager != null)
            //{
            //    if (!string.IsNullOrEmpty(telephonyManager.DeviceId)) { telephonyDeviceID = telephonyManager.DeviceId; }
            //    if (!string.IsNullOrEmpty(telephonyManager.SimSerialNumber)) { telephonySIMSerialNumber = telephonyManager.SimSerialNumber; }
            //}
            var androidID = Android.Provider.Settings.Secure.GetString(activity.ApplicationContext.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
            //var deviceUuid = new UUID(androidID.GetHashCode(), (telephonyDeviceID.GetHashCode() << 32) | telephonySIMSerialNumber.GetHashCode());
            return androidID;
        }

        public static string getDeviceName()
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            string manufacturer = Build.Manufacturer;
            string model = Build.Model;
            //Build.
            if (model.StartsWith(manufacturer))
            {
                return textInfo.ToTitleCase(model);
            }
            return textInfo.ToTitleCase(manufacturer) + " " + model;
        }

        public static void MensajeAdvertencia(Activity activity, string mensaje)
        {
            AlertDialog.Builder alert = new AlertDialog.Builder(activity);
            alert.SetTitle(activity.GetString(Resource.String.alerta_titulo_validacion));
            alert.SetMessage(mensaje);
            alert.SetPositiveButton(activity.GetString(Resource.String.alerta_boton_aceptar), (senderAlert, args) =>
            {

            });

            //alert.SetNegativeButton("Cancel", (senderAlert, args) => {
            //    Toast.MakeText(this, "Cancelled!", ToastLength.Short).Show();
            //});
            Dialog dialog = alert.Create();
            dialog.Show();
        }







        //public  static int MensajeCerrarUbicacion(Activity activity, string mensaje)
        //{
        //    var b = 0;
        //    int rest=0 ;

        //    if (Looper.MyLooper() == null)
        //        Looper.Prepare();
        //    try
        //    {

        //        AlertDialog.Builder alert = new AlertDialog.Builder(activity);
        //        alert.SetTitle(activity.GetString(Resource.String.alerta_titulo_validacion));
        //        alert.SetMessage(mensaje);
        //        alert.SetPositiveButton(activity.GetString(Resource.String.alerta_boton_aceptar), (senderAlert, args) =>
        //        {
        //            b = 1;

        //            rest = b;

        //        });


        //        //alert.SetNegativeButton("Cancel", (senderAlert, args) => {
        //        //    Toast.MakeText(this, "Cancelled!", ToastLength.Short).Show();
        //        //});

        //        return rest;
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

    }
}