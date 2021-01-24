using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PopCat
{
    public partial class PopCatWindow
    {
        private VolumeKeyListener _volumeKeyListener;
        private WindowResizeListener _windowResizeListener;
        private VolumeLevelListener _volumeLevelListener;
        
        private readonly IntPtr _hWnd;

        private readonly Timer _hideTimer;
        
        public PopCatWindow()
        {
            InitializeComponent();

            _hWnd = Win32Utils.FindOsdWindow();
            if (_hWnd == IntPtr.Zero)
            {
                Debug.WriteLine("No OSD window found");
                Application.Current.Shutdown();
                return;
            }
            
            Debug.WriteLine("Found window: " + _hWnd);
            
            _hideTimer = new Timer(_ => ShowWindow(false));
        }

        private void PopCatWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            _volumeKeyListener = new VolumeKeyListener();
            _volumeKeyListener.OnKeyPressed += OnVolumeKeyPress;
            _volumeKeyListener.Hook();

            _windowResizeListener = new WindowResizeListener(_hWnd);
            _windowResizeListener.OnResize += OnVolumeOSDResize;
            _windowResizeListener.Start();

            _volumeLevelListener = new VolumeLevelListener();
            _volumeLevelListener.OnVolumeLevelChange += OnVolumeLevelChange;
            _volumeLevelListener.Start();  
            
            ShowWindow(false);

            var (x, y) = Win32Utils.GetWindowPosition(_hWnd);
            var (width, height) = Win32Utils.GetWindowDimensions(_hWnd);
            
            ResizeVolumeOsd(x, y, width, height);
            
            Debug.WriteLine("Loaded window");
        }

        private void PopCatWindow_OnUnloaded(object sender, RoutedEventArgs e)
        {
            _volumeKeyListener.UnHook();
            _windowResizeListener.Stop();
            
            Debug.WriteLine("Unloaded window");
        }
        
        private void OnVolumeKeyPress()
        {
            ShowWindow(true);
            _hideTimer.Change(TimeSpan.FromSeconds(3), Timeout.InfiniteTimeSpan);
            
            Debug.WriteLine("Volume key pressed");
        }
        
        private void OnVolumeOSDResize(object sender, WindowResizeListener.WindowResizeEventArgs e)
        {
            ResizeVolumeOsd(e.X, e.Y, e.Width, e.Height);
            
            Debug.WriteLine("OSD resized");
        }
        
        private void OnVolumeLevelChange(object sender, VolumeLevelListener.VolumeLevelEventArgs e)
        {
            var key = Math.Ceiling(e.Level / 20.0).ToString("0");
            
            Dispatcher.Invoke(() =>
            {
                if (FindName("Image") is Image img && FindResource(key) is ImageSource src) img.Source = src;
            });
            
            Debug.WriteLine("Volume level changed");
        }

        private void ShowWindow(bool show)
        {
            Dispatcher.Invoke(() => Visibility = show ? Visibility.Visible : Visibility.Hidden);
        }

        private void ResizeVolumeOsd(int x, int y, int width, int height)
        {
            Dispatcher.Invoke(() =>
            {
                Width = height;
                Height = height;
                Left = x + width;
                Top = y;
            });
        }
    }
}