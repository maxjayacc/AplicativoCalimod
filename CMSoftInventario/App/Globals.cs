using Android.App;
using Android.Runtime;
using CMSoftInventario.App.Model;
using System;
using System.Collections.Generic;

namespace CMSoftInventario.App
{
    [Application]
    public class Globals : Application
    {
        public Globals(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {

        }

        public static Equipo Equipo { get; set; }
        public static Parametro Parametro { get; set; }
        public static List<KitItem> ListaItem { get; set; }

        public override void OnCreate()
        {
            base.OnCreate();
            Parametro = new Parametro();
        }
    }
}