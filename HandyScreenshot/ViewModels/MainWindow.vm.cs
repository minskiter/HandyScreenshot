﻿using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HandyScreenshot.Common;
using HandyScreenshot.Detection;
using HandyScreenshot.Helpers;
using HandyScreenshot.Mvvm;

namespace HandyScreenshot.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private static readonly byte[] SampleBytes = new byte[4];

        private readonly RectDetector _detector;
        private string _dpiString = string.Empty;

        public string DpiString
        {
            get => _dpiString;
            set => SetProperty(ref _dpiString, value);
        }

        public Func<int, int, Color> ColorGetter { get; }

        public BitmapSource Background { get; }

        public MonitorInfo MonitorInfo { get; }

        public ICommand CloseCommand { get; } = new RelayCommand(() => Application.Current.Shutdown(0));

        public ScreenshotState State { get; }

        public ICommand EnterCommand { get; set; } = new RelayCommand(() => { });

        public MainWindowViewModel(
            IObservable<(MouseMessage message, int x, int y)> mouseEventSource,
            BitmapSource background,
            MonitorInfo monitorInfo,
            RectDetector detector)
        {
            State = new ScreenshotState(DetectRectFromPhysicalPoint);
            Background = background;
            MonitorInfo = monitorInfo;
            _detector = detector;

            var disposable = mouseEventSource
                .Subscribe(i => State.PushState(i.message, i.x, i.y));

            ColorGetter = GetColorByCoordinate;

            SharedProperties.Disposables.Push(disposable);
            EnterCommand = new RelayCommand(() =>
            {
                //https://stackoverflow.com/questions/12257908/how-to-crop-image-and-save-into-imagesource-in-wpf
                if (State != null && Background != null)
                {
                    Clipboard.SetImage(new CroppedBitmap(Background, new Int32Rect(State.ScreenshotRect.X, State.ScreenshotRect.Y, State.ScreenshotRect.Width, State.ScreenshotRect.Height)));
                    Application.Current.Shutdown(1);
                }
            });
        }

        public void Initialize()
        {
            var initPoint = Win32Helper.GetPhysicalMousePosition();
            State.PushState(MouseMessage.MouseMove, initPoint.X, initPoint.Y);
        }

        private Color GetColorByCoordinate(int x, int y)
        {
            if (x < 0 || x >= Background.PixelWidth ||
                y < 0 || y >= Background.PixelHeight) return Colors.Transparent;

            Background.CopyPixels(new Int32Rect(x, y, 1, 1), SampleBytes, 4, 0);

            return Color.FromArgb(SampleBytes[3], SampleBytes[2], SampleBytes[1], SampleBytes[0]);
        }

        private ReadOnlyRect DetectRectFromPhysicalPoint(int physicalX, int physicalY)
        {
            var rect = _detector.GetByPhysicalPoint(physicalX, physicalY);
            return rect != ReadOnlyRect.Empty && MonitorInfo.PhysicalScreenRect.IntersectsWith(rect)
                ? rect
                : ReadOnlyRect.Zero;
        }
    }
}