using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;
using PInvoke;

namespace PopCat
{
    public class VolumeKeyListener
    {
        private static readonly Key[] VolumeKeys = {Key.VolumeUp, Key.VolumeDown, Key.VolumeMute};

        private readonly User32.WindowsHookDelegate _callback;
        private User32.SafeHookHandle _hookId = User32.SafeHookHandle.Null;

        public event Action OnKeyPressed;

        public VolumeKeyListener()
        {
            _callback = HookCallback;
        }

        public void Hook()
        {
            _hookId = SetHook(_callback);
        }

        public void UnHook()
        {
            _hookId.Close();
        }

        private static User32.SafeHookHandle SetHook(User32.WindowsHookDelegate proc)
        {
            using var curProcess = Process.GetCurrentProcess();
            using var curModule = curProcess.MainModule;
            
            var moduleHandle = Kernel32.GetModuleHandle(curModule?.ModuleName);
            
            return User32.SetWindowsHookEx(User32.WindowsHookType.WH_KEYBOARD_LL, proc,
                moduleHandle.DangerousGetHandle(), 0);
        }

        private int HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if ((nCode < 0 || wParam != (IntPtr) User32.WindowMessage.WM_KEYDOWN) &&
                wParam != (IntPtr) User32.WindowMessage.WM_SYSKEYDOWN)
                return User32.CallNextHookEx(_hookId.DangerousGetHandle(), nCode, wParam, lParam);

            var vkCode = Marshal.ReadInt32(lParam);

            var keyPressed = KeyInterop.KeyFromVirtualKey(vkCode);
            if (VolumeKeys.Contains(keyPressed))
            {
                OnKeyPressed?.Invoke();
            }

            return User32.CallNextHookEx(_hookId.DangerousGetHandle(), nCode, wParam, lParam);
        }
    }
}