using Android.Support.V4.App;
using Android.Views;
using Java.Lang;
using System.Collections.Generic;
using System.Linq;

namespace CMSoftInventario.App.Adapter
{
    public class TabsFragmentPagerAdapter : FragmentPagerAdapter
    {
        private readonly Android.Support.V4.App.Fragment[] fragments;
        private readonly ICharSequence[] titles;

        private Dictionary<int, string> fragmentTags;
        private FragmentManager fragmentManager;

        public TabsFragmentPagerAdapter(FragmentManager fm, Android.Support.V4.App.Fragment[] fragments, ICharSequence[] titles) : base(fm)
        {
            this.fragmentManager = fm;
            this.fragments = fragments;
            this.titles = titles;
            this.fragmentTags = new Dictionary<int, string>();
        }

        public override int Count
        {
            get { return fragments.Length; }
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            return fragments[position];
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return titles[position];
        }

        public override Object InstantiateItem(ViewGroup container, int position)
        {
            Object obj = base.InstantiateItem(container, position);
            if (obj is Android.Support.V4.App.Fragment)
            {
                Android.Support.V4.App.Fragment f = (Android.Support.V4.App.Fragment)obj;
                string tag = f.Tag;
                fragmentTags.Add(position, tag);
            }
            return obj;
        }

        public Android.Support.V4.App.Fragment GetFragment(int position)
        {
            KeyValuePair<int, string> dTag = fragmentTags.Where(x => x.Key == position).FirstOrDefault();
            if (string.IsNullOrEmpty(dTag.Value)) { return null; }
            return fragmentManager.FindFragmentByTag(dTag.Value);
        }

        public override void DestroyItem(ViewGroup container, int position, Object obj)
        {
            fragmentTags.Remove(position);
            base.DestroyItem(container, position, obj);
        }
    }
}