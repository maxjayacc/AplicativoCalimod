using Android.App;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using CMSoftInventario.App.Adapter;
using CMSoftInventario.App.Model;
using CMSoftInventario.App.Util;
using Symbol.XamarinEMDK;
using Symbol.XamarinEMDK.Barcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMSoftInventario.App.ViewModel
{
    public class LecturaKitViewModel : Java.Lang.Object, IDisposable, EMDKManager.IEMDKListener
    {

        #region "Controles"

        private Scanner scanner = null;

        private View dialogView;
        private AlertDialog kitDialog;
        private InputMethodManager imm;
        private TabLayout tabLayout;
        private ViewPager viewpager;

        private EditText etUbicacion;
        private EditText etKitReconteo;
        private TextView tvKitDesReconteo;
        private EditText etKit;
        private EditText etItem;
        private EditText etCantidad;
        private CheckBox chkUbicacion;
        private CheckBox chkKit;
        private CheckBox chkItem;
        private CheckBox chkReconteo;
        private Button btnGuardar;
        private TextView tvTotalKit;
        private TextView tvTotalItem;
        private Button btnCerrar;
        private Button btnSalir;
        private RecyclerView rvItem;
        private RecyclerView.LayoutManager rvItemLayout;
        private RecyclerView.Adapter rvInventarioAdapter;
        private TextView tvUltimo;
        private ImageView ivCancelar;


        #endregion

        #region "Variables"

        private Activity activity;
        private MemoryData memoryData;
        private Parametro parametro;
        private List<KitItem> lista = new List<KitItem>();
        private List<KitItem> listaItem = new List<KitItem>();
        private short lectura;

        private EMDKManager emdkManager = null;
        private BarcodeManager barcodeManager = null;
        private IList<ScannerInfo> scannerList = null;
        private int scannerIndex = 0;

        #endregion

        #region "Constructor"

        public LecturaKitViewModel(Activity activity, View dialogView, AlertDialog kitDialog)
        {
            this.activity = activity;
            this.dialogView = dialogView;
            this.kitDialog = kitDialog;
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

            etUbicacion = dialogView.FindViewById<EditText>(Resource.Id.lec_kit_ubi);
            etKitReconteo = dialogView.FindViewById<EditText>(Resource.Id.lec_kit_kit_reconteo);
            tvKitDesReconteo = dialogView.FindViewById<TextView>(Resource.Id.lec_kit_kit_desreconteo);
            etKit = dialogView.FindViewById<EditText>(Resource.Id.lec_kit_codigo);
            etItem = dialogView.FindViewById<EditText>(Resource.Id.lec_kit_item);
            etCantidad = dialogView.FindViewById<EditText>(Resource.Id.lec_kit_cantidad);
            chkUbicacion = dialogView.FindViewById<CheckBox>(Resource.Id.lec_kit_check_ubi);
            chkKit = dialogView.FindViewById<CheckBox>(Resource.Id.lec_kit_check_codigo);
            chkItem = dialogView.FindViewById<CheckBox>(Resource.Id.lec_kit_check_item);
            chkReconteo = dialogView.FindViewById<CheckBox>(Resource.Id.lec_kit_check_reconteo);
            btnGuardar = dialogView.FindViewById<Button>(Resource.Id.lec_kit_guardar);
            tvTotalKit = dialogView.FindViewById<TextView>(Resource.Id.lect_kit_total_kit);
            tvTotalItem = dialogView.FindViewById<TextView>(Resource.Id.lect_kit_total_item);
            btnCerrar = dialogView.FindViewById<Button>(Resource.Id.lec_kit_cerrar);
            btnSalir = dialogView.FindViewById<Button>(Resource.Id.lec_kit_salir);

            rvItem = dialogView.FindViewById<RecyclerView>(Resource.Id.recyclerViewKitItem);
            rvItemLayout = new LinearLayoutManager(activity, LinearLayoutManager.Vertical, false);
            rvItem.SetLayoutManager(rvItemLayout);

            tvUltimo = dialogView.FindViewById<TextView>(Resource.Id.lec_kit_ultimo);
            ivCancelar = dialogView.FindViewById<ImageView>(Resource.Id.lec_kit_cancelar);

            chkUbicacion.Click += (sender, e) => { chkUbicacion_Click(sender, e); };
            etUbicacion.EditorAction += (sender, e) => { etUbicacion_EditorAction(sender, e); };
            chkKit.Click += (sender, e) => { chkKit_Click(sender, e); };
            etKit.EditorAction += (sender, e) => { etKit_EditorAction(sender, e); };
            chkItem.Click += (sender, e) => { chkItem_Click(sender, e); };
            etItem.EditorAction += (sender, e) => { etItem_EditorAction(sender, e); };
            btnGuardar.Click += (sender, e) => { btnGuardar_Click(sender, e); };
            btnCerrar.Click += (sender, e) => { btnCerrar_Click(sender, e); };
            btnSalir.Click += (sender, e) => { btnSalir_Click(sender, e); };
            ivCancelar.Click += (sender, e) => { ivCancelar_Click(sender, e); };

            etUbicacion.FocusChange += (sender, e) => { etUbicacion_FocusChange(sender, e); };
            etKit.FocusChange += (sender, e) => { etItem_FocusChange(sender, e); };
            etItem.FocusChange += (sender, e) => { etItem_FocusChange(sender, e); };
        }

        private void chkUbicacion_Click(object sender, EventArgs e)
        {
            if (!chkUbicacion.Checked) { etUbicacion.Enabled = true; }
            else { etUbicacion.Enabled = false; }
        }

        private void etUbicacion_EditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            if (e.ActionId == ImeAction.Done) { ConfirmarUbicacion(); }
            else if (e.ActionId == ImeAction.Next) { ConfirmarUbicacion(); }
            else { e.Handled = false; }
        }

        private void etKit_EditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            if (e.ActionId != ImeAction.ImeNull)
            {
                if (listaItem == null) { listaItem = new List<KitItem>(); }
                if (e.ActionId == ImeAction.Done)
                {
                    if (!chkKit.Checked)
                    {
                        etItem.RequestFocus();
                        if (listaItem.Count > 0) { lista = new List<KitItem>(listaItem.Where(x => x.Codigo_Caja == etKit.Text && x.idAsignacionUbicacion == parametro.IdAsignacionUbicacion)); }
                    }
                }
                else if (e.ActionId == ImeAction.Next)
                {
                    if (!chkKit.Checked)
                    {
                        etItem.RequestFocus();
                        if (listaItem.Count > 0) { lista = new List<KitItem>(listaItem.Where(x => x.Codigo_Caja == etKit.Text && x.idAsignacionUbicacion == parametro.IdAsignacionUbicacion)); }
                    }
                }
                else { e.Handled = false; }

                rvInventarioAdapter = new ItemAdapter(activity, lista, rvItem);
                rvItem.SetAdapter(rvInventarioAdapter);
            }

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

        private void etUbicacion_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (((EditText)sender).Id == Resource.Id.lec_kit_ubi) { lectura = (short)Enumerador.eLectura.Ubicacion; }
        }

        private void etKit_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (((EditText)sender).Id == Resource.Id.lec_kit_codigo) { lectura = (short)Enumerador.eLectura.Kit; }
        }

        private void etItem_FocusChange(object sender, View.FocusChangeEventArgs e)
        {
            if (((EditText)sender).Id == Resource.Id.lec_kit_item) { lectura = (short)Enumerador.eLectura.Item; }
        }

        private void chkKit_Click(object sender, EventArgs e)
        {
            if (!chkKit.Checked)
            {
                etKit.Enabled = true;
                etKit.RequestFocus();
                lectura = (short)Enumerador.eLectura.Kit;
            }
            else { etKit.Enabled = false; }
        }

        private void chkItem_Click(object sender, EventArgs e)
        {
            if (!chkItem.Checked)
            {
                etItem.Enabled = true;
                etCantidad.Enabled = true;
                etItem.RequestFocus();
                Utilitarios.MensajeAdvertencia(activity, activity.GetString(Resource.String.lectura_advertencia_manual));
                lectura = (short)Enumerador.eLectura.Item;
            }
            else
            {
                etItem.Enabled = false;
                etCantidad.Enabled = false;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (lectura == (short)Enumerador.eLectura.Kit)
            {
                GuardarCaja();

                ToneGenerator toneGen1 = new ToneGenerator(Stream.Alarm, 5500);
                toneGen1.StartTone(Tone.CdmaPip, 1500);
            }

            else if (lectura == (short)Enumerador.eLectura.Item) { GuardarItem(); }
        }

        private void ivCancelar_Click(object sender, EventArgs e)
        {
            Borrar();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Cerrar();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            if (listaItem == null) { listaItem = new List<KitItem>(); }
            if (lista == null) { lista = new List<KitItem>(); }
            if (lista.Count > 0)
            {
                foreach (KitItem item in lista)
                {
                    if (listaItem.Count(x => x.Codigo_Caja == item.Codigo_Caja && x.Codigo_Subordinado == item.Codigo_Subordinado && x.idAsignacionUbicacion == parametro.IdAsignacionUbicacion) > 0)
                    {
                        foreach (KitItem itemGlobal in listaItem.Where(x => x.Codigo_Caja == item.Codigo_Caja && x.Codigo_Subordinado == item.Codigo_Subordinado && x.idAsignacionUbicacion == parametro.IdAsignacionUbicacion))
                        {
                            itemGlobal.Cantidad_Tomada = item.Cantidad_Tomada;
                        }
                    }
                    else { listaItem.Add(item); }
                }
                Globals.ListaItem = listaItem;
            }

            kitDialog.Dismiss();
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

                if (connectionState == BarcodeManager.ConnectionState.Disconnected) { DeInitScanner(); }
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
            try
            {
                StatusData statusData = e.P0;
                StatusData.ScannerStates state = e.P0.State;
                if (state != null)
                {
                    if (state == StatusData.ScannerStates.Idle)
                    {
                        Looper.Prepare();
                        try
                        {
                            if (scanner != null)
                            {
                                if (scanner.IsEnabled && !scanner.IsReadPending)
                                {
                                    SystemClock.Sleep(100);
                                    //Thread.Sleep(100);
                                    if (scanner != null) { scanner.Read(); }
                                }
                                else if (scanner.IsReadPending) { scanner.CancelRead(); }
                            }
                        }
                        catch (ScannerException ex) { Toast.MakeText(activity, ex.Message, ToastLength.Long).Show(); }
                        catch (NullReferenceException ex) { Toast.MakeText(activity, ex.StackTrace, ToastLength.Long).Show(); }
                        finally { Looper.MyLooper().Quit(); }
                    }
                    if (state == StatusData.ScannerStates.Error)
                    {
                        Toast.MakeText(activity, activity.GetString(Resource.String.lectura_error_mensaje), ToastLength.Long).Show();
                    }
                }
            }
            catch (ScannerException ex) { Toast.MakeText(activity, ex.Message, ToastLength.Long).Show(); }
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
        private void CargarDatosIniciales()
        {
            parametro = Globals.Parametro;
            lista = new List<KitItem>();
            if (Globals.ListaItem == null) { Globals.ListaItem = new List<KitItem>(); }
            listaItem = new List<KitItem>(Globals.ListaItem);

            if (listaItem.Count > 0)
            {
                if (listaItem.Count(x => x.Codigo_Caja == etKit.Text && x.idAsignacionUbicacion == parametro.IdAsignacionUbicacion) > 0) { chkItem.Enabled = false; }
                else { chkKit.Enabled = true; }
            }

            EMDKResults results = EMDKManager.GetEMDKManager(Application.Context, this);

            rvInventarioAdapter = new ItemAdapter(activity, lista, rvItem);
            rvItem.SetAdapter(rvInventarioAdapter);

            etUbicacion.Text = parametro.NombreUbicacion.Trim();
            etUbicacion.Focusable = true;
            etKit.Enabled = false;
            etItem.Enabled = false;
            etCantidad.Enabled = false;
            etCantidad.Text = "0";
            etKitReconteo.Enabled = false;
            etKitReconteo.Text = parametro.Reconteo;
            tvKitDesReconteo.Text = parametro.DesReconteo;

            if (parametro.Reconteo.Trim().Length > 0)
            {
                etKitReconteo.Visibility = ViewStates.Visible;
                tvKitDesReconteo.Visibility = ViewStates.Visible;
                chkReconteo.Visibility = ViewStates.Visible;
            }
            else
            {
                etKitReconteo.Visibility = ViewStates.Gone;
                tvKitDesReconteo.Visibility = ViewStates.Gone;
                chkReconteo.Visibility = ViewStates.Gone;
            }

            chkUbicacion.Checked = true;
            chkKit.Checked = true;
            chkItem.Checked = true;
            chkReconteo.Checked = true;

            tvUltimo.Text = parametro.UltimaLectura;
            if (string.IsNullOrEmpty(parametro.UltimaLectura)) { ivCancelar.Visibility = ViewStates.Gone; }
            else { ivCancelar.Visibility = ViewStates.Visible; }

            lectura = (short)Enumerador.eLectura.Ubicacion;
        }


        /// <summary>
        /// Guardar lectura kit
        /// </summary>
        private async void GuardarCaja()
        {
            string mensaje = string.Empty;
            if (string.IsNullOrEmpty(etUbicacion.Text)) { mensaje = string.Concat(mensaje, "- ", activity.GetString(Resource.String.lectura_advertencia_ubicacion), "\n"); }
            if (string.IsNullOrEmpty(etKit.Text)) { mensaje = string.Concat(mensaje, "- ", activity.GetString(Resource.String.lectura_advertencia_kit), "\n"); }
            if (etUbicacion.Text.Trim() == etKit.Text.Trim() && !string.IsNullOrEmpty(etUbicacion.Text) && !string.IsNullOrEmpty(etKit.Text)) { mensaje = string.Concat(mensaje, "- ", activity.GetString(Resource.String.lectura_advertencia_kit_ubicacion), "\n"); }

            if (parametro.Reconteo.Trim().Length > 0)
            {
                if (!etKit.Text.Contains(etKitReconteo.Text.Trim()) && chkReconteo.Checked) { mensaje = string.Concat(mensaje, "- ", activity.GetString(Resource.String.lectura_advertencia_kit_reconteo), "\n"); }
            }

            if (!string.IsNullOrEmpty(mensaje)) { Utilitarios.MensajeAdvertencia(activity, mensaje); }
            else
            {
                if (new Networks(activity).VerificaConexion())
                {
                    var progressDialog = ProgressDialog.Show(activity, activity.GetString(Resource.String.progress_titulo), activity.GetString(Resource.String.progress_mensaje_guardando), true);
                    try
                    {
                        memoryData = MemoryData.GetInstance(activity);
                        int idTerminal = memoryData.GetData(Constante.CMIdterminal);
                        int idUsuario = memoryData.GetData(Constante.CMIdUsuario);

                        KitCaja kit = new KitCaja();
                        kit.idUsuario = idUsuario;
                        kit.idEquipo = idTerminal;
                        kit.idInventario = parametro.IdInventario;
                        kit.idUbicacion = parametro.IdUbicacion;
                        kit.idAsignacionUbicacion = parametro.IdAsignacionUbicacion;
                        kit.Codigo_Caja = etKit.Text;
                        if (chkItem.Checked) { kit.Es_Automatico = "1"; }
                        else { kit.Es_Automatico = "0"; }
                        kit.Estado = Constante.EstadoKit;
                        kit.Cantidad_Tomada = 1;

                        Proxy.ProxyGestorTx gestorTx = new Proxy.ProxyGestorTx();
                        string resul = await gestorTx.LecturaItemCaja(kit);
                        if (resul.ToLower() == Constante.RespuestaExito.ToLower())
                        {
                            Snackbar.Make(dialogView, Resource.String.lectura_correcto, Snackbar.LengthLong).Show();
                            tvUltimo.Text = etKit.Text;
                            tvUltimo.SetTextColor(Color.Black);
                            ivCancelar.Visibility = ViewStates.Visible;
                            etCantidad.Enabled = true;
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
                            etCantidad.Enabled = false;
                            etItem.Enabled = false;
                            chkItem.Enabled = false;
                            tvUltimo.Text = etKit.Text;
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
        /// Guardar lectura item
        /// </summary>
        private async void GuardarItem()
        {
            string mensaje = string.Empty;
            if (string.IsNullOrEmpty(etUbicacion.Text)) { mensaje = string.Concat(mensaje, "- ", activity.GetString(Resource.String.lectura_advertencia_ubicacion), "\n"); }
            if (string.IsNullOrEmpty(etKit.Text)) { mensaje = string.Concat(mensaje, "- ", activity.GetString(Resource.String.lectura_advertencia_kit), "\n"); }
            if (string.IsNullOrEmpty(etItem.Text)) { mensaje = string.Concat(mensaje, "- ", activity.GetString(Resource.String.lectura_advertencia_item), "\n"); }
            if (decimal.Parse(etCantidad.Text) <= 0) { mensaje = string.Concat(mensaje, "- ", activity.GetString(Resource.String.lectura_advertencia_cantidad), "\n"); }
            if (etUbicacion.Text.Trim() == etItem.Text.Trim() && !string.IsNullOrEmpty(etUbicacion.Text) && !string.IsNullOrEmpty(etItem.Text)) { mensaje = string.Concat(mensaje, "- ", activity.GetString(Resource.String.lectura_advertencia_item_ubicacion), "\n"); }
            if (etKit.Text.Trim() == etItem.Text.Trim() && !string.IsNullOrEmpty(etKit.Text) && !string.IsNullOrEmpty(etItem.Text)) { mensaje = string.Concat(mensaje, "- ", activity.GetString(Resource.String.lectura_advertencia_kit_item), "\n"); }

            if (!string.IsNullOrEmpty(mensaje)) { Utilitarios.MensajeAdvertencia(activity, mensaje); }
            else
            {
                if (new Networks(activity).VerificaConexion())
                {
                    var progressDialog = ProgressDialog.Show(activity, activity.GetString(Resource.String.progress_titulo), activity.GetString(Resource.String.progress_mensaje_guardando), true);
                    try
                    {
                        memoryData = MemoryData.GetInstance(activity);
                        int idTerminal = memoryData.GetData(Constante.CMIdterminal);
                        int idUsuario = memoryData.GetData(Constante.CMIdUsuario);

                        Kit kit = new Kit();
                        kit.idUsuario = idUsuario;
                        kit.idEquipo = idTerminal;
                        kit.idInventario = parametro.IdInventario;
                        kit.idUbicacion = parametro.IdUbicacion;
                        kit.idAsignacionUbicacion = parametro.IdAsignacionUbicacion;
                        kit.Codigo_Caja = etKit.Text;
                        kit.Codigo_Subordinado = etItem.Text;
                        if (chkItem.Checked) { kit.Es_Automatico = "1"; }
                        else { kit.Es_Automatico = "0"; }
                        kit.Estado = Constante.EstadoKit;
                        kit.Cantidad_Tomada = decimal.Parse(etCantidad.Text);


                        Proxy.ProxyGestorTx gestorTx = new Proxy.ProxyGestorTx();

                        string resul = await gestorTx.LecturaItemKit(kit);

                        if (resul.ToLower() == Constante.RespuestaExito.ToLower())

                        {
                            chkKit.Enabled = false;
                            Snackbar.Make(dialogView, Resource.String.lectura_correcto, Snackbar.LengthLong).Show();
                            tvUltimo.Text = etKit.Text;
                            tvUltimo.SetTextColor(Color.Black);
                            ivCancelar.Visibility = ViewStates.Visible;

                            if (lista.Count(x => x.Codigo_Caja == kit.Codigo_Caja && x.Codigo_Subordinado == kit.Codigo_Subordinado && x.idAsignacionUbicacion == kit.idAsignacionUbicacion) > 0)
                            {

                                foreach (KitItem item in lista.Where(x => x.Codigo_Caja == kit.Codigo_Caja && x.Codigo_Subordinado == kit.Codigo_Subordinado && x.idAsignacionUbicacion == kit.idAsignacionUbicacion))
                                {

                                    item.Cantidad_Tomada = item.Cantidad_Tomada + kit.Cantidad_Tomada;

                                }

                            }
                            else
                            {
                                lista.Add(new KitItem()
                                {

                                    Codigo_Caja = kit.Codigo_Caja,
                                    Codigo_Subordinado = kit.Codigo_Subordinado,
                                    idUsuario = kit.idUsuario,
                                    idInventario = kit.idInventario,
                                    idUbicacion = kit.idUbicacion,
                                    idAsignacionUbicacion = kit.idAsignacionUbicacion,
                                    Cantidad_Tomada = decimal.Parse(etCantidad.Text)

                                });
                            }

                            rvInventarioAdapter = new ItemAdapter(activity, lista, rvItem);
                            rvItem.SetAdapter(rvInventarioAdapter);

                            etItem.Text = string.Empty;
                            etCantidad.Text = "0";
                            etItem.RequestFocus();
                            imm.HideSoftInputFromWindow(dialogView.WindowToken, HideSoftInputFlags.None);
                            lectura = (short)Enumerador.eLectura.Item;
                            ObtenerTotal();

                            ToneGenerator toneGen1 = new ToneGenerator(Stream.Alarm, 100);
                            toneGen1.StartTone(Constante.TonoCorrecto);
                            SystemClock.Sleep(100);
                            toneGen1.Release();

                        }
                        else
                        {
                            tvUltimo.Text = etKit.Text;
                            tvUltimo.SetTextColor(Color.Red);
                            ivCancelar.Visibility = ViewStates.Gone;
                            Snackbar.Make(dialogView, Resource.String.lectura_incorrecto, Snackbar.LengthLong).Show();

                            ToneGenerator toneGen1 = new ToneGenerator(Stream.Alarm, 100);
                            toneGen1.StartTone(Constante.TonoInCorrecto);
                            toneGen1.Release();
                        }


                    }     //scan = true;

                    catch (ApplicationException) { Snackbar.Make(dialogView, Resource.String.app_error, Snackbar.LengthLong).Show(); }
                    catch (NullReferenceException) { Snackbar.Make(dialogView, Resource.String.app_error, Snackbar.LengthLong).Show(); }
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
            if (string.IsNullOrEmpty(tvUltimo.Text)) { mensaje = string.Concat(mensaje, "- ", activity.GetString(Resource.String.lectura_advertencia_kit), "\n"); }

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

                        Kit kit = new Kit();
                        kit.idUsuario = idUsuario;
                        kit.idEquipo = idTerminal;
                        kit.idInventario = parametro.IdInventario;
                        kit.idUbicacion = parametro.IdUbicacion;
                        kit.idAsignacionUbicacion = parametro.IdAsignacionUbicacion;
                        kit.Codigo_Caja = tvUltimo.Text;
                        kit.Codigo_Subordinado = string.Empty;
                        kit.Es_Automatico = string.Empty;
                        kit.Estado = Constante.EstadoKit;
                        kit.Cantidad_Tomada = 0;

                        Proxy.ProxyGestorTx gestorTx = new Proxy.ProxyGestorTx();
                        string resul = await gestorTx.BorrarLecturaItemKitUnitario(kit);
                        if (resul.ToLower() == Constante.RespuestaExito.ToLower())
                        {
                            Snackbar.Make(dialogView, Resource.String.lectura_borrar_correcto, Snackbar.LengthLong).Show();
                            tvUltimo.Text = string.Empty;
                            ivCancelar.Visibility = ViewStates.Gone;

                            lista = new List<KitItem>();
                            listaItem.RemoveAll(x => x.Codigo_Caja == etKit.Text && x.idAsignacionUbicacion == parametro.IdAsignacionUbicacion);
                            Globals.ListaItem = listaItem;
                            rvInventarioAdapter = new ItemAdapter(activity, lista, rvItem);
                            rvItem.SetAdapter(rvInventarioAdapter);

                            etKit.Text = string.Empty;
                            etKit.RequestFocus();
                            etItem.Text = string.Empty;
                            chkItem.Checked = true;
                            etCantidad.Text = "0";
                            chkKit.Enabled = true;
                            lectura = (short)Enumerador.eLectura.Kit;
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
        /// Cerrar kit
        /// </summary>
        private async void Cerrar()
        {
            string mensaje = string.Empty;
            if (string.IsNullOrEmpty(etUbicacion.Text)) { mensaje = string.Concat(mensaje, "- ", activity.GetString(Resource.String.lectura_advertencia_ubicacion), "\n"); }
            if (string.IsNullOrEmpty(etKit.Text)) { mensaje = string.Concat(mensaje, "- ", activity.GetString(Resource.String.lectura_advertencia_kit), "\n"); }
            if (lista.Count == 0) { mensaje = string.Concat(mensaje, "- ", activity.GetString(Resource.String.lectura_advertencia_items), "\n"); }

            if (!string.IsNullOrEmpty(mensaje)) { Utilitarios.MensajeAdvertencia(activity, mensaje); }
            else
            {
                if (new Networks(activity).VerificaConexion())
                {
                    var progressDialog = ProgressDialog.Show(activity, activity.GetString(Resource.String.progress_titulo), activity.GetString(Resource.String.progress_mensaje_cerrando), true);
                    try
                    {
                        memoryData = MemoryData.GetInstance(activity);
                        int idTerminal = memoryData.GetData(Constante.CMIdterminal);
                        int idUsuario = memoryData.GetData(Constante.CMIdUsuario);

                        Kit kit = new Kit();
                        kit.idUsuario = idUsuario;
                        kit.idEquipo = idTerminal;
                        kit.idInventario = parametro.IdInventario;
                        kit.idUbicacion = parametro.IdUbicacion;
                        kit.idAsignacionUbicacion = parametro.IdAsignacionUbicacion;
                        kit.Codigo_Caja = etKit.Text;
                        kit.Codigo_Subordinado = string.Empty;
                        kit.Es_Automatico = string.Empty;
                        kit.Estado = Constante.EstadoKit;
                        kit.Cantidad_Tomada = 0;

                        Proxy.ProxyGestorTx gestorTx = new Proxy.ProxyGestorTx();
                        ResulKit resul = await gestorTx.CerrarLecturaItemKitUnitario(kit);
                        if (resul.Valor == Constante.ResultadoValorCorrecto)
                        {
                            Snackbar.Make(dialogView, Resource.String.lectura_kit_cerrar_correcto, Snackbar.LengthLong).Show();
                            tvUltimo.Text = etKit.Text;
                            ivCancelar.Visibility = ViewStates.Visible;

                            lista = new List<KitItem>();
                            listaItem.RemoveAll(x => x.Codigo_Caja == etKit.Text && x.idAsignacionUbicacion == parametro.IdAsignacionUbicacion);
                            Globals.ListaItem = listaItem;
                            rvInventarioAdapter = new ItemAdapter(activity, lista, rvItem);
                            rvItem.SetAdapter(rvInventarioAdapter);

                            etKit.Text = string.Empty;
                            chkKit.Checked = true;
                            etKit.Enabled = true;
                            etKit.RequestFocus();
                            imm.HideSoftInputFromWindow(dialogView.WindowToken, HideSoftInputFlags.None);
                            etItem.Text = string.Empty;
                            etCantidad.Text = "0";
                            chkItem.Checked = true;
                            etCantidad.Enabled = false;
                            etItem.Enabled = false;
                            chkKit.Enabled = true;
                            lectura = (short)Enumerador.eLectura.Kit;
                            ObtenerTotal();
                        }
                        else
                        {
                            AlertDialog.Builder alert = new AlertDialog.Builder(activity);
                            alert.SetTitle(activity.GetString(Resource.String.alerta_titulo_validacion));
                            alert.SetMessage(string.Concat(activity.GetString(Resource.String.lectura_kit_cerrar_correcto), " pero ", resul.ValidacionKit));
                            alert.SetPositiveButton(activity.GetString(Resource.String.alerta_boton_aceptar), (senderAlert, args) =>
                            {
                                lista = new List<KitItem>();
                                listaItem.RemoveAll(x => x.Codigo_Caja == etKit.Text && x.idAsignacionUbicacion == parametro.IdAsignacionUbicacion);
                                Globals.ListaItem = listaItem;
                                rvInventarioAdapter = new ItemAdapter(activity, lista, rvItem);
                                rvItem.SetAdapter(rvInventarioAdapter);

                                etKit.Text = string.Empty;
                                chkKit.Checked = true;
                                etKit.Enabled = true;
                                etKit.RequestFocus();
                                imm.HideSoftInputFromWindow(dialogView.WindowToken, HideSoftInputFlags.None);
                                etItem.Text = string.Empty;
                                etCantidad.Text = "0";
                                chkItem.Checked = true;
                                etCantidad.Enabled = false;
                                etItem.Enabled = false;
                                lectura = (short)Enumerador.eLectura.Kit;
                                ObtenerTotal();
                            });

                            Dialog dialog = alert.Create();
                            dialog.Show();
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
        /// Confirmar ubicación
        /// </summary>
        private async void ConfirmarUbicacion()
        {
            //if (scanner != null)
            //{
            //    if (scanner.IsEnabled)
            //    {
            //        scanner.CancelRead();
            //        scanner.Disable();
            //    }
            //}

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
                            etKit.Enabled = true;
                            chkKit.Enabled = true;
                            etKit.RequestFocus();
                            imm.HideSoftInputFromWindow(dialogView.WindowToken, HideSoftInputFlags.None);
                            chkItem.Enabled = true;
                            etItem.Text = string.Empty;
                            etCantidad.Text = "0";
                            if (chkItem.Checked)
                            {
                                etItem.Enabled = false;
                                etCantidad.Enabled = false;
                            }
                            else
                            {
                                etItem.Enabled = true;
                                etCantidad.Enabled = true;
                            }
                            lectura = (short)Enumerador.eLectura.Kit;

                            ToneGenerator toneGen1 = new ToneGenerator(Stream.Alarm, 100);
                            toneGen1.StartTone(Constante.TonoCorrecto);
                            SystemClock.Sleep(100);
                            toneGen1.Release();
                        }
                        else
                        {
                            etItem.Enabled = false;
                            chkItem.Enabled = false;
                            etKit.Enabled = false;
                            chkKit.Enabled = false;
                            etItem.Enabled = false;
                            etCantidad.Enabled = false;
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
                Proxy.ProxyGestorRx gestorRx = new Proxy.ProxyGestorRx();
                Totales total = await gestorRx.ObtenerTotalesTomas(parametro.IdAsignacionUbicacion);
                if (total != null)
                {
                    tvTotalItem.Text = total.Cantidad_Tomada_Pares.ToString("###,###,##0.0");
                    tvTotalKit.Text = total.Cantidad_Tomada_Cajas.ToString("###,###,##0.0");
                }
                else
                {
                    tvTotalItem.Text = "0.0";
                    tvTotalKit.Text = "0.0";
                }
            }
            catch (ApplicationException) { Snackbar.Make(dialogView, Resource.String.app_error, Snackbar.LengthLong).Show(); }
            catch (Exception) { Snackbar.Make(dialogView, Resource.String.app_error, Snackbar.LengthLong).Show(); }
        }


        private void InitScanner()
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
                        Task.WaitAll();
                    }
                    catch (ScannerException ex) { Toast.MakeText(activity, ex.StackTrace, ToastLength.Long).Show(); }
                    catch (NullReferenceException ex) { Toast.MakeText(activity, ex.StackTrace, ToastLength.Long).Show(); }
                }
            }
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
                        Task.WaitAll();
                    }
                }
                catch (ScannerException ex) { Toast.MakeText(activity, ex.StackTrace, ToastLength.Long).Show(); }
                catch (NullReferenceException ex) { Toast.MakeText(activity, ex.StackTrace, ToastLength.Long).Show(); }

                scanner.Data -= scanner_Data;
                scanner.Status -= scanner_Status;

                try
                {
                    scanner.Release();
                    Task.WaitAll();
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
                    //config.ScanParams.DecodeAudioFeedbackUri = "";
                    scanner.SetConfig(config);
                    Task.WaitAll();
                }
                catch (ScannerException ex) { Toast.MakeText(activity, ex.StackTrace, ToastLength.Long).Show(); }
            }
        }

        private void DisplayScanData(string data)
        {
            //scan = true;
            if (lectura == (short)Enumerador.eLectura.Ubicacion)
            {
                etUbicacion.Text = data;
                ConfirmarUbicacion();
            }
            else if (lectura == (short)Enumerador.eLectura.Kit)
            {
                etKit.Text = data;
                if (listaItem == null) { listaItem = new List<KitItem>(); }
                if (listaItem.Count > 0) { lista = new List<KitItem>(listaItem.Where(x => x.Codigo_Caja == etKit.Text && x.idAsignacionUbicacion == parametro.IdAsignacionUbicacion)); }
                rvInventarioAdapter = new ItemAdapter(activity, lista, rvItem);
                rvItem.SetAdapter(rvInventarioAdapter);
                GuardarCaja();
                Task.WaitAll();
            }
            else if (lectura == (short)Enumerador.eLectura.Item)
            {
                etItem.Text = data;
                etCantidad.Text = "1";

                if (chkItem.Checked)
                {
                    GuardarItem();
                    Task.WaitAll();
                }
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
                Task.WaitAll();
            }

            if (emdkManager != null) { emdkManager.Release(EMDKManager.FEATURE_TYPE.Barcode); }
        }

        #endregion
    }
}