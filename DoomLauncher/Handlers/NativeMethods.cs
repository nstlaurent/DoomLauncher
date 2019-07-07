using System;
using System.Runtime.InteropServices;

namespace DoomLauncher
{
    public class NativeMethods
    {
        public enum GWL
        {
            ExStyle = -20
        }

        public enum LWA
        {
            ColorKey = 0x1,
            Alpha = 0x2
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern int GetWindowLongNative(IntPtr hWnd, GWL nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLongNative(IntPtr hWnd, GWL nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetLayeredWindowAttributes")]
        private static extern bool SetLayeredWindowAttributesNative(IntPtr hWnd, int crKey, int alpha, LWA dwFlags);

        [DllImport("user32.dll", EntryPoint= "MoveWindow", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
        private static extern void MoveWindowNative(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

#pragma warning disable S4200
        public static int GetWindowLong(IntPtr hWnd, GWL nIndex) => GetWindowLongNative(hWnd, nIndex);
        public static int SetWindowLong(IntPtr hWnd, GWL nIndex, int dwNewLong) => SetWindowLongNative(hWnd, nIndex, dwNewLong);
        public static bool SetLayeredWindowAttributes(IntPtr hWnd, int crKey, int alpha, LWA dwFlags) => SetLayeredWindowAttributesNative(hWnd, crKey, alpha, dwFlags);
        public static void MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint) => MoveWindowNative(hwnd, X, Y, nWidth, nHeight, bRepaint);
    }
}
