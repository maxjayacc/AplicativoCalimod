package md54a523ccb170a33af52c4cdc36eb9edc3;


public class LecturaKitViewModel
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.symbol.emdk.EMDKManager.EMDKListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onClosed:()V:GetOnClosedHandler:Symbol.XamarinEMDK.EMDKManager/IEMDKListenerInvoker, Symbol.XamarinEMDK\n" +
			"n_onOpened:(Lcom/symbol/emdk/EMDKManager;)V:GetOnOpened_Lcom_symbol_emdk_EMDKManager_Handler:Symbol.XamarinEMDK.EMDKManager/IEMDKListenerInvoker, Symbol.XamarinEMDK\n" +
			"";
		mono.android.Runtime.register ("CMSoftInventario.App.ViewModel.LecturaKitViewModel, CMSoftInventario", LecturaKitViewModel.class, __md_methods);
	}


	public LecturaKitViewModel ()
	{
		super ();
		if (getClass () == LecturaKitViewModel.class)
			mono.android.TypeManager.Activate ("CMSoftInventario.App.ViewModel.LecturaKitViewModel, CMSoftInventario", "", this, new java.lang.Object[] {  });
	}

	public LecturaKitViewModel (android.app.Activity p0, android.view.View p1, android.app.AlertDialog p2)
	{
		super ();
		if (getClass () == LecturaKitViewModel.class)
			mono.android.TypeManager.Activate ("CMSoftInventario.App.ViewModel.LecturaKitViewModel, CMSoftInventario", "Android.App.Activity, Mono.Android:Android.Views.View, Mono.Android:Android.App.AlertDialog, Mono.Android", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public void onClosed ()
	{
		n_onClosed ();
	}

	private native void n_onClosed ();


	public void onOpened (com.symbol.emdk.EMDKManager p0)
	{
		n_onOpened (p0);
	}

	private native void n_onOpened (com.symbol.emdk.EMDKManager p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
