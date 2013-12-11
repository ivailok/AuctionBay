using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AuctionBay.AttachedProperties
{
    public class NavigationProperties
    {
        public static bool GetIsBackButtonEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsBackButtonEnabledProperty);
        }

        public static void SetIsBackButtonEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsBackButtonEnabledProperty, value);
        }

        public static readonly DependencyProperty IsBackButtonEnabledProperty =
            DependencyProperty.RegisterAttached("IsBackButtonEnabled", typeof(bool), typeof(UserControl), new PropertyMetadata(true, (snd, args) =>
            {
                var element = snd as UserControl;
                if (element == null)
                {
                    return;
                }

                var navigationPanel = element.Content as Panel;
                if (navigationPanel == null)
                {
                    return;
                }

                if (navigationPanel.Children == null)
                {
                    return;
                }
                if (navigationPanel.Children.Count == 0)
                {
                    return;
                }

                var leftNavigationPanel = navigationPanel.Children.First() as Panel;
                if (leftNavigationPanel == null)
                {
                    return;
                }
                if (leftNavigationPanel.Children == null)
                {
                    return;
                }
                if (leftNavigationPanel.Children.Count == 0)
                {
                    return;
                }

                var backButton = leftNavigationPanel.Children.First();
                var isEnabled = (bool)args.NewValue;
                if (isEnabled)
                {
                    backButton.Visibility = Visibility.Visible;
                }
                else
                {
                    backButton.Visibility = Visibility.Collapsed;
                }
            }));
    }
}
