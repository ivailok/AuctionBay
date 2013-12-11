using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AuctionBay.Behavior
{
    public class UICaptureButtonBehavior
    {
        public static ICommand GetOpenCamera(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(OpenCameraProperty);
        }

        public static void SetOpenCamera(DependencyObject obj, ICommand value)
        {
            obj.SetValue(OpenCameraProperty, value);
        }

        public static readonly DependencyProperty OpenCameraProperty =
            DependencyProperty.RegisterAttached("OpenCamera",
            typeof(ICommand),
            typeof(UICaptureButtonBehavior),
            new PropertyMetadata(null, new PropertyChangedCallback(ExecuteOpenCameraCommand)));

        private static void ExecuteOpenCameraCommand(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var clickedButton = d as Button;

            if (clickedButton == null)
            {
                return;
            }

            clickedButton.Click += async (sender, args) =>
            {
                var camera = new Windows.Media.Capture.CameraCaptureUI();
                camera.PhotoSettings.Format = Windows.Media.Capture.CameraCaptureUIPhotoFormat.Jpeg;
                camera.PhotoSettings.MaxResolution = Windows.Media.Capture.CameraCaptureUIMaxPhotoResolution.HighestAvailable;
                var image = await camera.CaptureFileAsync(Windows.Media.Capture.CameraCaptureUIMode.Photo);
                if (image != null)
                {
                    List<IStorageFile> container = new List<IStorageFile>() { image };
                    var command = (sender as UIElement).GetValue(UICaptureButtonBehavior.OpenCameraProperty) as ICommand;

                    try
                    {
                        command.Execute(image);
                    }
                    catch
                    {
                        command.Execute(container);
                    }
                }
            };
        }
    }
}
