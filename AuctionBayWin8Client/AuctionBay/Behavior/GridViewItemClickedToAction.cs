using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AuctionBay.Behavior
{
    public static class GridViewItemClickedToAction
    {
        public static DependencyProperty ActionProperty =
            DependencyProperty.RegisterAttached("Action", typeof(ICommand), typeof(GridViewItemClickedToAction), new PropertyMetadata(null, OnActionChanged));

        public static void SetAction(DependencyObject sender, ICommand action)
        {
            if (sender == null)
            {
                return;
            }

            sender.SetValue(ActionProperty, action);
        }

        public static ICommand GetAction(DependencyObject sender)
        {
            if (sender == null)
            {
                return null;
            }

            return (ICommand)sender.GetValue(ActionProperty);
        }

        private static void OnActionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ListViewBase gridView = (ListViewBase)d;

            if (gridView != null)
            {
                gridView.ItemClick += GridView_ItemClick;
                gridView.Unloaded += GridView_Unloaded;
            }
        }

        static void GridView_Unloaded(object sender, RoutedEventArgs e)
        {
            ListViewBase gridView = (ListViewBase)sender;
            gridView.ItemClick -= GridView_ItemClick;
            gridView.Unloaded -= GridView_Unloaded;
        }

        static void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var gridView = (ListViewBase)sender;
            ICommand action = (ICommand)gridView.GetValue(ActionProperty);
            action.Execute(e.ClickedItem);
        }
    }
}
