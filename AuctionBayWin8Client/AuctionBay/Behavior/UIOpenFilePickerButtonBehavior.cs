using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AuctionBay.Behavior
{
    public class UIOpenFilePickerButtonBehavior
    {
        public static ICommand GetOpenFilePickerMulti(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(OpenFilePickerMultiProperty);
        }

        public static void SetOpenFilePickerMulti(DependencyObject obj, ICommand value)
        {
            obj.SetValue(OpenFilePickerMultiProperty, value);
        }

        public static ICommand GetOpenFilePickerSingle(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(OpenFilePickerSingleProperty);
        }

        public static void SetOpenFilePickerSingle(DependencyObject obj, ICommand value)
        {
            obj.SetValue(OpenFilePickerSingleProperty, value);
        }

        // Using a DependencyProperty as the backing store for OpenFilePicker.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OpenFilePickerMultiProperty =
            DependencyProperty.RegisterAttached("OpenFilePickerMulti", 
            typeof(ICommand), 
            typeof(UIOpenFilePickerButtonBehavior), 
            new PropertyMetadata(null, new PropertyChangedCallback(ExecuteOpenFilePickerMultiCommand)));

        public static readonly DependencyProperty OpenFilePickerSingleProperty =
            DependencyProperty.RegisterAttached("OpenFilePickerSingle",
            typeof(ICommand),
            typeof(UIOpenFilePickerButtonBehavior),
            new PropertyMetadata(null, new PropertyChangedCallback(ExecuteOpenFilePickerSingleCommand)));

        private static void ExecuteOpenFilePickerMultiCommand(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var clickedButton = d as Button;

            if (clickedButton == null)
            {
                return;
            }

            clickedButton.Click += async (sender, args) =>
            {
                var picker = new FileOpenPicker();
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".png");
                picker.CommitButtonText = "Load images";
                picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                var files = await picker.PickMultipleFilesAsync();
                if (files != null)
                {
                    clickedButton.IsEnabled = false;
                    var command = (sender as UIElement).GetValue(UIOpenFilePickerButtonBehavior.OpenFilePickerMultiProperty) as ICommand;
                    command.Execute(files);
                    clickedButton.IsEnabled = true;
                }
            };
        }

        private static void ExecuteOpenFilePickerSingleCommand(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var clickedButton = d as Button;

            if (clickedButton == null)
            {
                return;
            }

            clickedButton.Click += async (sender, args) =>
            {
                var picker = new FileOpenPicker();
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".png");
                picker.CommitButtonText = "Load image";
                picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                var file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    var command = (sender as UIElement).GetValue(UIOpenFilePickerButtonBehavior.OpenFilePickerSingleProperty) as ICommand;
                    command.Execute(file);
                }
            };
        }
    }
}
