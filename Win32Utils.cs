using System;
using PInvoke;

namespace PopCat
{
    public static class Win32Utils
    {
        public static Tuple<int, int> GetWindowDimensions(IntPtr hWnd)
        {
            User32.GetWindowRect(hWnd, out var rect);
            return new Tuple<int, int>(rect.right - rect.left, rect.bottom - rect.top);
        }

        public static Tuple<int, int> GetWindowPosition(IntPtr hWnd)
        {
            User32.GetWindowRect(hWnd, out var rect);
            return new Tuple<int, int>(rect.left, rect.top);
        }

        public static void HideControls(IntPtr hWnd, int width, int height)
        {
            User32.SetWindowPos(hWnd, IntPtr.Zero, 0, 0, width, height, User32.SetWindowPosFlags.SWP_NOMOVE);
        }

        public static IntPtr FindOsdWindow()
        {
            var hwnd = IntPtr.Zero;

            while ((hwnd = User32.FindWindowEx(IntPtr.Zero, hwnd, "NativeHWNDHost", "")) != IntPtr.Zero)
            {
                if (User32.FindWindowEx(hwnd, IntPtr.Zero, "DirectUIHWND", "") == IntPtr.Zero) continue;

                break;
            }

            return hwnd;
        }
    }
}