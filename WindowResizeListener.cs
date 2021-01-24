using System;
using System.Timers;

namespace PopCat
{
    public class WindowResizeListener
    {
        private readonly IntPtr _hWnd;
        private Tuple<int, int> _size;
        private readonly Timer _timer;
        
        public EventHandler<WindowResizeEventArgs> OnResize;

        public WindowResizeListener(IntPtr hWnd)
        {
            _hWnd = hWnd;
            _size = Win32Utils.GetWindowDimensions(_hWnd);
            
            _timer = new Timer() {Interval = 10};
            _timer.Elapsed += (_, _) => CheckResize();
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void CheckResize()
        {
            var currentSize = Win32Utils.GetWindowDimensions(_hWnd);
            var (width, height) = currentSize;
            var (x, y) = Win32Utils.GetWindowPosition(_hWnd);
            
            if (Equals(currentSize, _size)) return;
            
            _size = currentSize;
            var args = new WindowResizeEventArgs()
            {
                Width = width,
                Height = height,
                X = x,
                Y = y
            };
            
            OnResize.Invoke(this, args);
        }

        public class WindowResizeEventArgs : EventArgs
        {
            public int Width { get; init; }
            public int Height { get; init; }
            public int X { get; init; }
            public int Y { get; init; }
        }
    }
}