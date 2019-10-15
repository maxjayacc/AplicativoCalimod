using Android.App;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using CMSoftInventario.App.Model;
using CMSoftInventario.App.Util;
using Symbol.XamarinEMDK;
using Symbol.XamarinEMDK.Barcode;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CMSoftInventario.App.ViewModel
{
    public class LecturaItemViewModel : Java.Lang.Object, IDisposable, EMDKManager.IEMDKListener
    {

        #region "Controles"

        private Scanner scanner = null;

        private InputMethodManager imm;



        private View dialogView;
        private AlertDialog itemDialog;
        private TabLayout tabLayout;
        private ViewPager viewpager;

        private Button btnSalir;
        private Button btnGuardar;
        private EditText etUbicacion;
        private EditText etItemReconteo;
        private TextView tvItemDesReconteo;
        private EditText etItem;
        private EditText etCantidad;
        private CheckBox chkUbicacion;
        private CheckBox chkItem;
        private CheckBox chkReconteo;
        private TextView tvUltimo;
        private TextView tvTotal;
        private ImageView ivCancelar;

        #endregion

        #region "Variables"

        private MemoryData memoryData;
        private Parametro parametro;
        private Activity activity;
        private short lectura;

        private EMDKManager emdkManager = null;
        private BarcodeManager barcodeManager = null;
        private IList<ScannerInfo> scannerList = null;
        private int scannerIndex = 0;
        private bool scan = false;

        #endregion

        #region "Constructor"

        public LecturaItemViewModel(Activity activity, View dialogView, AlertDialog itemDialog)
        {
            this.activity = activity;
            this.dialogView = dialogView;
            this.itemDialog = itemDialog;
            Inicializar();
            CargarDatosIniciales();
        }

        #endregion

        #region "Eventos"

        private void Inicializar()
        {
            tabLayout = activity.FindViewById<TabLayout>(Resource.Id.sliding_tabs);
            viewpager = activity.FindViewById<ViewPager>(Resource.Id.viewpager);

            imm = (InputMethodManager)activity.BaseContext.GetSystemService(Android.Content.Context.InputMethodService);
            imm.HideSoftInputFromWindow(dialogView.WindowToken, HideSoftInputFlags.None);

            btnSalir = dialogView.FindViewById<Button>(Resource.Id.lec_item_salir);
            btnGuardar = dialogView.FindViewById<Button>(Resource.Id.lec_item_guardar);
            etUbicacion = dialogView.FindViewById<EditText>(Resource.Id.lec_item_ubi);
            etItemReconteo = dialogView.FindViewById<EditText>(Resource.Id.lec_item_item_reconteo);
            tvItemDesReconteo = dialogView.FindViewById<TextView>(Resource.Id.lec_item_item_desreconteo);
            etItem = dialogView.FindViewById<EditText>(Resource.Id.lec_item_item);
            etCantidad = dialogView.FindViewById<EditText>(Resource.Id.lec_item_cantidad);
            chkUbicacion = dialogView.FindViewById<CheckBox>(Resource.Id.lec_check_ubi);
            chkItem = dialogView.FindViewById<CheckBox>(Resource.Id.lec_check_item);
            chkReconteo = dialogView.FindViewById<CheckBox>(Resource.Id.lec_check_reconteo);
            tvUltimo = dialogView.FindViewById<TextView>(Resource.Id.lec_item_ultimo);
            tvTotal = dialogView.FindViewById<TextView>(Resource.Id.lect_item_total);
            ivCancelar = dialogView.FindViewById<ImageView>(Resource.Id.lec_item_cancelar);

            chkUbicacion.Click += (sender, args) => { chkUbicacion_Click(sender, args); };
            etUbicacion.EditorAction += (sender, e) => { etUbicacion_EditorAction(sender, e); };
            chkItem.Click += (sender, e) => { chkItem_Click(sender, e); };
            etItem.EditorAction += (sender, e) => { etItem_EditorAction(sender, e); };
            btnGuardar.Click += (sender, e) => { btnGuardar_Click(sender, e); };
            btnSalir.Click += (sender, e) => { btnSalir_Click(sender, e); };
            ivCancelar.Click += (sender, e) => { ivCancelar_Click(sender, e); };

            etUbicacion.FocusChange += (sender, e) => { etUbicacion_FocusChange(sender, e); };
            etItem.FocusChange += (sender, e) => { etItem_FocusChange(sender, e); };
        }

        private void chkUbicacion_Click(object sender, EventArgs e)
        {
            etUbicacion.RequestFocus();
            if (!chkUbicacion.Checked) { imm.ToggleSoftInputFromWindow(dialogView.WindowToken, ShowSoftInputFlags.None, HideSoftInputFlags.None); }
            else { imm.HideSoftInputFromWindow(dialogView.WindowToken, HideSoftInputFlags.None); }
        }

        private void etUbicacion_EditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            if (e.ActionId == ImeAction.Done) { ConfirmarUbicacion(); }
            else if (e.ActionId == ImeAction.Next) { ConfirmarUbicacion(); }
            else { e.Handled = false; }
        }

        private void etUbicacion_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (((EditText)sender).Id == Resource.Id.lec_item_ubi) { lectura = (short)Enumerador.eLectura.Ubicacion; }
        }

        private void etItem_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (((EditText)sender).Id == Resource.Id.lec_item_item) { lectura = (short)Enumerador.eLectura.Item; }
        }

        private void etItem_EditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            if (e.ActionId == ImeAction.Done)
            {
                if (!chkItem.Checked) { etCantidad.RequestFocus(); }
            }
            else if (e.ActionId == ImeAction.Next)
            {
                if (!chkItem.Checked) { etCantidad.RequestFocus(); }
            }
            else { e.Handled = false; }
        }

        private void chkItem_Click(object sender, EventArgs e)
        {
            if (!chkItem.Checked)
            {
                etItem.Enabled = true;
                etCantidad.Enabled = true;
                Utilitarios.MensajeAdvertencia(activity, activity.GetString(Resource.String.lectura_advertencia_manual));
            }
            else
            {
                etItem.Enabled = false;
                etCantidad.Enabled = false;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            Guardar();
        }

        private void ivCancelar_Click(object sender, EventArgs e)
        {
            Borrar();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            itemDialog.Dismiss();
        }

        private void barcodeManager_Connection(object sender, BarcodeManager.ScannerConnectionEventArgs e)
        {
            string scannerName = string.Empty;

            ScannerInfo scannerInfo = e.P0;
            BarcodeManager.ConnectionState connectionState = e.P1;

            string statusBT = connectionState.ToString();
            string scannerNameBT = scannerInfo.FriendlyName;

            if (scannerList.Count != 0) { scannerName = scannerList[scannerIndex].FriendlyName; }

            if (scannerName.ToLower().Equals(scannerNameBT.ToLower()))
            {
                if (connectionState == BarcodeManager.ConnectionState.Connected)
                {
                    DeInitScanner();
                    InitScanner();
                    SetConfig();
                }

                if (connectionState == BarcodeManager.ConnectionState.Disconnected)
                {
                    DeInitScanner();
                }
            }
        }

        private void scanner_Data(object sender, Scanner.DataEventArgs e)
        {
            ScanDataCollection scanDataCollection = e.P0;

            if ((scanDataCollection != null) && (scanDataCollection.Result == ScannerResults.Success))
            {
                IList<ScanDataCollection.ScanData> scanData = scanDataCollection.GetScanData();

                foreach (ScanDataCollection.ScanData data in scanData)
                {
                    string dataString = data.Data;
                    activity.RunOnUiThread(() => DisplayScanData(dataString));
                }
            }
        }

        private void scanner_Status(object sender, Scanner.StatusEventArgs e)
        {
            StatusData statusData = e.P0;
            StatusData.ScannerStates state = e.P0.State;



            if (state == StatusData.ScannerStates.Idle)
            {
                Looper.Prepare();
                try
                {
                    if (scanner.IsEnabled && !scanner.IsReadPending)
                    {
                        SystemClock.Sleep(100);
                        //Thread.Sleep(100);
                        if (scanner != null) { scanner.Read(); }
                    }
                    else if (scanner.IsReadPending) { scanner.CancelRead(); }
                }
                catch (ScannerException ex) { Toast.MakeText(activity, ex.Message, ToastLength.Long).Show(); }
                catch (NullReferenceException ex) { Toast.MakeText(activity, ex.StackTrace, ToastLength.Long).Show(); }
                finally { Looper.MyLooper().Quit(); }
            }

            if (state == StatusData.ScannerStates.Error)
            {
                scan = true;
                Toast.MakeText(activity, activity.GetString(Resource.String.lectura_error_mensaje), ToastLength.Long).Show();
            }
        }

        public void OnOpened(EMDKManager emdkManagerInstance)
        {
            this.emdkManager = emdkManagerInstance;

            try
            {
                barcodeManager = (BarcodeManager)emdkManager.GetInstance(EMDKManager.FEATURE_TYPE.Barcode);

                if (barcodeManager != null)
                {
                    barcodeManager.Connection += barcodeManager_Connection;
                    scannerList = barcodeManager.SupportedDevicesInfo;
                    if (scanner == null) { InitScanner(); }
                }
            }
            catch (Exception ex) { Toast.MakeText(activity, ex.StackTrace, ToastLength.Long).Show(); }
        }

        public void OnClosed()
        {

        }

        #endregion

        #region "Metodos Privados"

        /// <summary>
        /// Cargar datos iniciales
        /// </summary>
        public void CargarDatosIniciales()
        {
            scan = true;
            EMDKResults results = EMDKManager.GetEMDKManager(Application.Context, this);

            parametro = Globals.Parametro;
            memoryData = MemoryData.GetInstance(activity);

            ObtenerTotal();

            etUbicacion.Text = parametro.NombreUbicacion.Trim();
            etUbicacion.Focusable = true;
            imm.HideSoftInputFromWindow(etUbicacion.WindowToken, 0);

            etItem.Enabled = false;
            etCantidad.Enabled = false;
            etCantidad.Text = "0";
            etItemReconteo.Enabled = false;
            etItemReconteo.Text = parametro.Reconteo;
            tvItemDesReconteo.Text = parametro.DesReconteo;

            if (parametro.Reconteo.Trim().Length > 0)
            {
                etItemReconteo.Visibility = ViewStates.Visible;
                tvItemDesReconteo.Visibility = ViewStates.Visible;
                chkReconteo.Visibility = ViewStates.Visible;
            }
            else
            {
                etItemReconteo.Visibility = ViewStates.Gone;
                tvItemDesReconteo.Visibility = ViewStates.Gone;
                chkReconteo.Visibility = ViewStates.Gone;
            }

            chkUbicacion.Checked = true;
            chkItem.Checked = true;
            chkReconteo.Checked = true;

            tvUltimo.Text = parametro.UltimaLectura;
            if (string.IsNullOrEmpty(parametro.UltimaLectura)) { ivCancelar.Visibility = ViewStates.Gone; }
            else { ivCancelar.Visibility = ViewStates.Visible; }

            lectura = (short)Enumerador.eLectura.Ubicacion;

        }

        /// <summary>
        /// Guardar lectura
        /// </summary>
        private async void Guardar()
        {
            string mensaje = string.Empty;
            if (string.IsNullOrEmpty(etUbicacion.Text)) { mensaje = string.Concat(mensaje, "- ", activity.GetString(Resource.String.lectura_advertencia_ubicacion), "\n"); }
            if (string.IsNullOrEmpty(etItem.Text)) { mensaje = string.Concat(mensaje, "- ", activity.GetString(Resource.String.lectura_advertencia_item), "\n"); }
            if (decimal.Parse(etCantidad.Text) <= 0) { mensaje = string.Concat(mensaje, "- ", activity.GetString(Resource.String.lectura_advertencia_cantidad), "\n"); }
            if (etUbicacion.Text.Trim() == etItem.Text.Trim()) { mensaje = string.Concat(mensaje, "- ", activity.GetString(Resource.String.lectura_advertencia_item_ubicacion), "\n"); }
            if (parametro.Reconteo.Trim().Length > 0)
            {
                if (!etItem.Text.Contains(etItemReconteo.Text.Trim()) && chkReconteo.Checked) { mensaje = string.Concat(mensaje, "- ", activity.GetString(Resource.String.lectura_advertencia_item_reconteo), "\n"); }
            }

            if (!string.IsNullOrEmpty(mensaje)) { Utilitarios.MensajeAdvertencia(activity, mensaje); }
            else
            {
                if (new Networks(activity).VerificaConexion())
                {
                    var progressDialog = ProgressDialog.Show(activity, activity.GetString(Resource.String.progress_titulo), activity.GetString(Resource.String.progress_mensaje_guardando), true);
                    try
                    {
                        int idTerminal = memoryData.GetData(Constante.CMIdterminal);
                        int idUsuario = memoryData.GetData(Constante.CMIdUsuario);

                        Item item = new Item();
                        item.idUsuario = idUsuario;
                        item.idEquipo = idTerminal;
                        item.idInventario = parametro.IdInventario;
                        item.idUbicacion = parametro.IdUbicacion;
                        item.idAsignacionUbicacion = parametro.IdAsignacionUbicacion;
                        item.CodBarraItemUnitario = etItem.Text;
                        if (chkItem.Checked) { item.Es_Automatico = "1"; }
                        else { item.Es_Automatico = "0"; }
                        item.Cantidad_Tomada = decimal.Parse(etCantidad.Text);

                        Proxy.ProxyGestorTx gestorTx = new Proxy.ProxyGestorTx();
                        string resul = await gestorTx.LecturaItemUnitario(item);
                        if (resul.ToLower() == Constante.RespuestaExito.ToLower())
                        {
                            Snackbar.Make(dialogView, Resource.String.lectura_correcto, Snackbar.LengthLong).Show();
                            tvUltimo.Text = etItem.Text;
                            tvUltimo.SetTextColor(Color.Black);
                            ivCancelar.Visibility = ViewStates.Visible;

                            etItem.Text = string.Empty;
                            etCantidad.Text = "0";
                            etItem.RequestFocus();
                            lectura = (short)Enumerador.eLectura.Item;
                            ObtenerTotal();

                            ToneGenerator toneGen1 = new ToneGenerator(Stream.Alarm, 100);
                            toneGen1.StartTone(Constante.TonoCorrecto);
                            SystemClock.Sleep(100);
                            toneGen1.Release();
                        }
                        else
                        {
                            tvUltimo.Text = etItem.Text;
                            tvUltimo.SetTextColor(Color.Red);
                            ivCancelar.Visibility = ViewStates.Gone;
                            Snackbar.Make(dialogView, Resource.String.lectura_incorrecto, Snackbar.LengthLong).Show();

                            ToneGenerator toneGen1 = new ToneGenerator(Stream.Alarm, 100);
                            toneGen1.StartTone(Constante.TonoInCorrecto);
                            SystemClock.Sleep(100);
                            toneGen1.Release();
                        }
                        //scan = true;
                    }
                    catch (ApplicationException) { Snackbar.Make(dialogView, Resource.String.app_error, Snackbar.LengthLong).Show(); }
                    catch (Exception) { Snackbar.Make(dialogView, Resource.String.app_error, Snackbar.LengthLong).Show(); }
                    finally { progressDialog.Hide(); }
                }
                else { Snackbar.Make(dialogView, Resource.String.app_network, Snackbar.LengthLong).Show(); }
            }
        }

        /// <summary>
        /// Borrar última lectura
        /// </summary>
        private async void Borrar()
        {
            string mensaje = string.Empty;
            if (string.IsNullOrEmpty(etUbicacion.Text)) { mensaje = string.Concat(mensaje, "- ", activity.GetString(Resource.String.lectura_advertencia_ubicacion), "\n"); }
            if (string.IsNullOrEmpty(tvUltimo.Text)) { mensaje = string.Concat(mensaje, "- ", activity.GetString(Resource.String.lectura_advertencia_item), "\n"); }

            if (!string.IsNullOrEmpty(mensaje)) { Utilitarios.MensajeAdvertencia(activity, mensaje); }
            else
            {
                if (new Networks(activity).VerificaConexion())
                {
                    var progressDialog = ProgressDialog.Show(activity, activity.GetString(Resource.String.progress_titulo), activity.GetString(Resource.String.progress_mensaje_borrando), true);
                    try
                    {
                        memoryData = MemoryData.GetInstance(activity);
                        int idTerminal = memoryData.GetData(Constante.CMIdterminal);
                        int idUsuario = memoryData.GetData(Constante.CMIdUsuario);

                        Item item = new Item();
                        item.idUsuario = idUsuario;
                        item.idEquipo = idTerminal;
                        item.idInventario = parametro.IdInventario;
                        item.idUbicacion = parametro.IdUbicacion;
                        item.idAsignacionUbicacion = parametro.IdAsignacionUbicacion;
                        item.CodBarraItemUnitario = tvUltimo.Text;
                        item.Es_Automatico = string.Empty;
                        item.Cantidad_Tomada = decimal.Parse(etCantidad.Text);

                        Proxy.ProxyGestorTx gestorTx = new Proxy.ProxyGestorTx();
                        string resul = await gestorTx.BorrarLecturaItemUnitario(item);
                        if (resul == Constante.RespuestaExito)
                        {
                            Snackbar.Make(dialogView, Resource.String.lectura_borrar_correcto, Snackbar.LengthLong).Show();
                            tvUltimo.Text = string.Empty;
                            ivCancelar.Visibility = ViewStates.Gone;
                            etItem.Text = string.Empty;
                            etCantidad.Text = "0";
                            etItem.RequestFocus();
                            lectura = (short)Enumerador.eLectura.Item;
                            ObtenerTotal();
                        }
                        else { Snackbar.Make(dialogView, Resource.String.lectura_borrar_incorrecto, Snackbar.LengthLong).Show(); }
                    }
                    catch (ApplicationException) { Snackbar.Make(dialogView, Resource.String.app_error, Snackbar.LengthLong).Show(); }
                    catch (Exception) { Snackbar.Make(dialogView, Resource.String.app_error, Snackbar.LengthLong).Show(); }
                    finally { progressDialog.Hide(); }
                }
                else { Snackbar.Make(dialogView, Resource.String.app_network, Snackbar.LengthLong).Show(); }
            }
        }

        /// <summary>
        /// Confirmar ubicación
        /// </summary>
        private async void ConfirmarUbicacion()
        {

            string mensaje = string.Empty;
            if (string.IsNullOrEmpty(etUbicacion.Text)) { mensaje = string.Concat(mensaje, "- ", activity.GetString(Resource.String.lectura_advertencia_ubicacion), "\n"); }
            if (etUbicacion.Text.Trim() != parametro.NombreUbicacion.Trim())
            {
                mensaje = string.Concat(mensaje, "- ", activity.GetString(Resource.String.lectura_advertencia_conf_ubi_dif), "\n");

                ToneGenerator toneGen1 = new ToneGenerator(Stream.Alarm, 100);
                toneGen1.StartTone(Constante.TonoInCorrecto);
                SystemClock.Sleep(100);
                toneGen1.Release();


            }

            if (!string.IsNullOrEmpty(mensaje)) { Utilitarios.MensajeAdvertencia(activity, mensaje); }
            else
            {
                if (new Networks(activity).VerificaConexion())
                {
                    var progressDialog = ProgressDialog.Show(activity, activity.GetString(Resource.String.progress_titulo), activity.GetString(Resource.String.progress_mensaje_confirmando), true);
                    try
                    {
                        memoryData = MemoryData.GetInstance(activity);
                        int idTerminal = memoryData.GetData(Constante.CMIdterminal);
                        int idUsuario = memoryData.GetData(Constante.CMIdUsuario);

                        Proxy.ProxyGestorRx gestorRx = new Proxy.ProxyGestorRx();
                        Confirmacion confUbicacion = await gestorRx.ConfirmarUbicacion(idUsuario, idTerminal, parametro.IdInventario, etUbicacion.Text);
                        if (confUbicacion.Valor == Constante.ResultadoValorCorrecto)
                        {
                            Snackbar.Make(dialogView, Resource.String.lectura_confirmar_correcto, Snackbar.LengthLong).Show();
                            etItem.Enabled = true;
                            chkItem.Enabled = true;
                            etItem.Text = string.Empty;
                            etCantidad.Text = "0";
                            etItem.RequestFocus();
                            imm.HideSoftInputFromWindow(dialogView.WindowToken, HideSoftInputFlags.None);
                            lectura = (short)Enumerador.eLectura.Item;

                            ToneGenerator toneGen1 = new ToneGenerator(Stream.Alarm, 100);
                            toneGen1.StartTone(Constante.TonoCorrecto);
                            SystemClock.Sleep(100);
                            toneGen1.Release();

                        }
                        else
                        {
                            etItem.Enabled = false;
                            chkItem.Enabled = false;
                            etUbicacion.RequestFocus();
                            Snackbar.Make(dialogView, Resource.String.lectura_confirmar_incorrecto, Snackbar.LengthLong).Show();

                            ToneGenerator toneGen1 = new ToneGenerator(Stream.Alarm, 100);
                            toneGen1.StartTone(Constante.TonoInCorrecto);
                            SystemClock.Sleep(100);
                            toneGen1.Release();
                        }
                    }
                    catch (ApplicationException) { Snackbar.Make(dialogView, Resource.String.app_error, Snackbar.LengthLong).Show(); }
                    catch (Exception) { Snackbar.Make(dialogView, Resource.String.app_error, Snackbar.LengthLong).Show(); }
                    finally { progressDialog.Hide(); }
                }
                else { Snackbar.Make(dialogView, Resource.String.app_network, Snackbar.LengthLong).Show(); }
            }
        }

        /// <summary>
        /// Obtener total de lecturas
        /// </summary>
        private async void ObtenerTotal()
        {
            try
            {
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.CurrencyDecimalSeparator = ".";
                nfi.NumberDecimalSeparator = ".";
                nfi.CurrencyGroupSeparator = ",";

                Proxy.ProxyGestorRx gestorRx = new Proxy.ProxyGestorRx();
                Totales total = await gestorRx.ObtenerTotalesTomas(parametro.IdAsignacionUbicacion);
                if (total != null) { tvTotal.Text = total.Cantidad_Tomada_Pares.ToString("###,###,##0.0", nfi); }
                else { tvTotal.Text = "0.0"; }
            }
            catch (ApplicationException) { Snackbar.Make(dialogView, Resource.String.app_error, Snackbar.LengthLong).Show(); }
            catch (Exception) { Snackbar.Make(dialogView, Resource.String.app_error, Snackbar.LengthLong).Show(); }
        }

        private void InitScanner()
        {
            try
            {
                if (scanner == null)
                {
                    if ((scannerList != null) && (scannerList.Count > 0)) { scanner = barcodeManager.GetDevice(scannerList[scannerIndex]); }
                    else { return; }

                    if (scanner != null)
                    {
                        scanner.Data += scanner_Data;
                        scanner.Status += scanner_Status;

                        try
                        {
                            scanner.Enable();
                            scanner.Read();
                        }
                        catch (ScannerException ex) { Toast.MakeText(activity, ex.StackTrace, ToastLength.Long).Show(); }
                        catch (NullReferenceException ex) { Toast.MakeText(activity, ex.StackTrace, ToastLength.Long).Show(); }
                    }
                }

            }
            catch (ScannerException ex) { Toast.MakeText(activity, ex.StackTrace, ToastLength.Long).Show(); }
            catch (NullReferenceException ex) { Toast.MakeText(activity, ex.StackTrace, ToastLength.Long).Show(); }
        }

        private void DeInitScanner()
        {
            if (scanner != null)
            {
                try
                {
                    if (scanner.IsEnabled)
                    {
                        scanner.CancelRead();
                        scanner.Disable();
                    }
                }
                catch (ScannerException ex) { Toast.MakeText(activity, ex.StackTrace, ToastLength.Long).Show(); }
                catch (NullReferenceException ex) { Toast.MakeText(activity, ex.StackTrace, ToastLength.Long).Show(); }

                scanner.Data -= scanner_Data;
                scanner.Status -= scanner_Status;

                try
                {
                    scanner.Release();
                }
                catch (ScannerException ex) { Toast.MakeText(activity, ex.StackTrace, ToastLength.Long).Show(); }
                catch (NullReferenceException ex) { Toast.MakeText(activity, ex.StackTrace, ToastLength.Long).Show(); }
                scanner = null;
            }
        }

        private void SetConfig()
        {
            if (scanner == null) { InitScanner(); }

            if ((scanner != null) && (scanner.IsEnabled))
            {
                try
                {
                    scanner.TriggerType = Scanner.TriggerTypes.Hard;
                    ScannerConfig config = scanner.GetConfig();
                    config.DecoderParams.Ean8.Enabled = true;
                    config.DecoderParams.Ean13.Enabled = true;
                    config.DecoderParams.Code39.Enabled = true;
                    config.DecoderParams.Code128.Enabled = true;
                    scanner.SetConfig(config);
                }
                catch (ScannerException ex) { Toast.MakeText(activity, ex.StackTrace, ToastLength.Long).Show(); }
            }
        }

        private void DisplayScanData(string data)
        {
            scan = true;
            if (lectura == (short)Enumerador.eLectura.Ubicacion)
            {
                etUbicacion.Text = data;
                ConfirmarUbicacion();
            }
            else if (lectura == (short)Enumerador.eLectura.Item)
            {
                etItem.Text = data;
                etCantidad.Text = "1";

                if (chkItem.Checked) { Guardar(); }
                else
                {
                    etCantidad.RequestFocus();
                    etCantidad.SelectAll();
                }
            }
        }

        #endregion

        #region "Metodos Publicos"

        public void DestroyScanner()
        {
            DeInitScanner();

            if (barcodeManager != null)
            {
                barcodeManager.Connection -= barcodeManager_Connection;
                barcodeManager = null;
                scannerList = null;
            }

            if (emdkManager != null) { emdkManager.Release(EMDKManager.FEATURE_TYPE.Barcode); }
        }

        #endregion



    }

}