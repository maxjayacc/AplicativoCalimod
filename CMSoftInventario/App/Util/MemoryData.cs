using Android.Content;
using Android.Preferences;
using Android.Support.V7.App;

namespace CMSoftInventario.App.Util
{
    public class MemoryData : AppCompatActivity
    {
        private static MemoryData memoryData;
        private ISharedPreferences sharedPreferences;
        private ISharedPreferencesEditor prefesEditor;

        public MemoryData(Context context)
        {
            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(context);
        }
        public static MemoryData GetInstance(Context context)
        {
            if (memoryData == null) { memoryData = new MemoryData(context); }
            return memoryData;
        }
        public void SaveData(string key, int value)
        {
            prefesEditor = sharedPreferences.Edit();
            prefesEditor.PutInt(key, value);
            prefesEditor.Commit();
        }

        public void SaveDataString(string key, string value)
        {
            prefesEditor = sharedPreferences.Edit();
            prefesEditor.PutString(key, value);
            prefesEditor.Commit();
        }

        public int GetData(string key)
        {
            if (sharedPreferences != null) { return sharedPreferences.GetInt(key, 0); }
            return 0;
        }

        public string GetDataString(string key)
        {
            if (sharedPreferences != null) { return sharedPreferences.GetString(key, string.Empty); }
            return string.Empty;
        }
    }
}