using Android.App;
using Android.Content;
using Android.Net;
using Android.Support.V7.App;

namespace CMSoftInventario.App.Util
{
    public class Networks : AppCompatActivity
    {
        private Activity _activity;
        public Networks(Activity activity)
        {
            _activity = activity;
        }

        public bool VerificaConexion()
        {
            bool valor = false;
            ConnectivityManager cm = (ConnectivityManager)_activity.GetSystemService(Context.ConnectivityService);
            NetworkInfo activeNetwork = cm.ActiveNetworkInfo;
            if (activeNetwork != null)
            {
                if (activeNetwork.Type == ConnectivityType.Wifi) { valor = true; }
                else if (activeNetwork.Type == ConnectivityType.Mobile) { valor = true; }
            }
            else { valor = false; }
            return valor;
        }
    }
}