package md55c259d0600f560c66ba055b75cdc397d;


public class EquipoDialogFragment
	extends android.app.DialogFragment
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreateDialog:(Landroid/os/Bundle;)Landroid/app/Dialog;:GetOnCreateDialog_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("CMSoftInventario.App.Fragment.EquipoDialogFragment, CMSoftInventario", EquipoDialogFragment.class, __md_methods);
	}


	public EquipoDialogFragment ()
	{
		super ();
		if (getClass () == EquipoDialogFragment.class)
			mono.android.TypeManager.Activate ("CMSoftInventario.App.Fragment.EquipoDialogFragment, CMSoftInventario", "", this, new java.lang.Object[] {  });
	}

	public EquipoDialogFragment (android.app.Activity p0)
	{
		super ();
		if (getClass () == EquipoDialogFragment.class)
			mono.android.TypeManager.Activate ("CMSoftInventario.App.Fragment.EquipoDialogFragment, CMSoftInventario", "Android.App.Activity, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public android.app.Dialog onCreateDialog (android.os.Bundle p0)
	{
		return n_onCreateDialog (p0);
	}

	private native android.app.Dialog n_onCreateDialog (android.os.Bundle p0);

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
