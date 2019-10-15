package md5cbe2dfae2c2fd36e184c628dd5c9b10f;


public class MemoryData
	extends android.support.v7.app.AppCompatActivity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("CMSoftInventario.App.Util.MemoryData, CMSoftInventario", MemoryData.class, __md_methods);
	}


	public MemoryData ()
	{
		super ();
		if (getClass () == MemoryData.class)
			mono.android.TypeManager.Activate ("CMSoftInventario.App.Util.MemoryData, CMSoftInventario", "", this, new java.lang.Object[] {  });
	}

	public MemoryData (android.content.Context p0)
	{
		super ();
		if (getClass () == MemoryData.class)
			mono.android.TypeManager.Activate ("CMSoftInventario.App.Util.MemoryData, CMSoftInventario", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
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
