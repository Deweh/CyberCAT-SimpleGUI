using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CP2077SaveEditor.Extensions
{
    public static class ListViewExtensions
    {
        public static void RetrieveDefaultVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = ((ListView)sender).GetVirtualInfo().Items[e.ItemIndex];
        }

        private static void DefaultColumnClick(object sender, ColumnClickEventArgs e)
        {
            ((ListView)sender).SortVirtualItems(e.Column);
        }

        public static void SetVirtualItems(this ListView targetView, List<ListViewItem> newList)
        {
            if (targetView.Tag == null)
            {
                targetView.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(targetView, true, null);
                targetView.RetrieveVirtualItem += RetrieveDefaultVirtualItem;
                targetView.ColumnClick += DefaultColumnClick;
                targetView.Tag = new VirtualInfo(newList);
            }
            else
            {
                targetView.GetVirtualInfo().Items = newList;
            }
            
            targetView.VirtualListSize = newList.Count();
            targetView.SortVirtualItems();
        }

        public static void RemoveVirtualItem(this ListView targetView, ListViewItem targetItem)
        {
            targetView.GetVirtualInfo().Items.Remove(targetItem);
            targetView.VirtualListSize -= 1;
            targetView.SortVirtualItems();
        }

        public static List<ListViewItem> SelectedVirtualItems(this ListView targetView)
        {
            var selectedList = new List<ListViewItem>();
            foreach (int i in targetView.SelectedIndices)
            {
                selectedList.Add(targetView.Items[i]);
            }
            return selectedList;
        }

        public static void SortVirtualItems(this ListView targetView, int column = -1)
        {
            var info = targetView.GetVirtualInfo();
            var comparer = new CaseInsensitiveComparer();

            int Compare(ListViewItem x, ListViewItem y)
            {
                int compareResult;

                if (int.TryParse(x.SubItems[info.SortColumn].Text, out _) && int.TryParse(y.SubItems[info.SortColumn].Text, out _))
                {
                    compareResult = comparer.Compare(int.Parse(y.SubItems[info.SortColumn].Text), int.Parse(x.SubItems[info.SortColumn].Text));
                }
                else
                {
                    compareResult = comparer.Compare(x.SubItems[info.SortColumn].Text, y.SubItems[info.SortColumn].Text);
                }

                if (info.SortOrder == SortOrder.Ascending)
                {
                    return compareResult;
                }
                else if (info.SortOrder == SortOrder.Descending)
                {
                    return (-compareResult);
                }
                else
                {
                    return 0;
                }
            }
            
            if (column > -1)
            {
                if (column == info.SortColumn)
                {
                    if (info.SortOrder == SortOrder.Ascending)
                    {
                        info.SortOrder = SortOrder.Descending;
                    }
                    else
                    {
                        info.SortOrder = SortOrder.Ascending;
                    }
                }
                else
                {
                    info.SortColumn = column;
                    info.SortOrder = SortOrder.Ascending;
                }
            }

            if (info.SortColumn > -1 && info.SortOrder != SortOrder.None)
            {
                info.Items.Sort((x, y) => Compare(x, y));
            }
            targetView.Update();
        }

        public static VirtualInfo GetVirtualInfo(this ListView targetView)
        {
            return (VirtualInfo)targetView.Tag;
        }

        public class VirtualInfo
        {
            public List<ListViewItem> Items { get; set; } = new List<ListViewItem>();
            public int SortColumn { get; set; } = -1;
            public SortOrder SortOrder { get; set; } = SortOrder.None;

            public VirtualInfo() { }

            public VirtualInfo(List<ListViewItem> _items)
            {
                Items = _items;
            }
        }
    }

    public static class StringExtensions
    {
        public static string LastOrIndex(this string[] arr, int position)
        {
            if (position == -1)
            {
                return arr.Last();
            } else {
                return arr[position];
            }
        }
    }
}
