package md5144fca257835e6172c3d5efcb9f1734e;


public class InventarioAdapterViewHolder
	extends android.support.v7.widget.RecyclerView.ViewHolder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("CMSoftInventario.App.Adapter.InventarioAdapterViewHolder, CMSoftInventario", InventarioAdapterViewHolder.class, __md_methods);
	}


	public InventarioAdapterViewHolder (android.view.View p0)
	{
		super (p0);
		if (getClass () == InventarioAdapterViewHolder.class)
			mono.android.TypeManager.Activate ("CMSoftInventario.App.Adapter.InventarioAdapterViewHolder, CMSoftInventario", "Android.Views.View, Mono.Android", this, new java.lang.Object[] { p0 });
	}

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
