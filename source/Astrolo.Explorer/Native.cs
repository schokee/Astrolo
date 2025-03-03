using System.Runtime.InteropServices;
using System.Windows.Interop;
using Newtonsoft.Json;

// ReSharper disable InconsistentNaming

namespace Astrolo.Explorer;

public static class Native
{
    private const int SW_SHOWNORMAL = 1;
    private const int SW_SHOWMINIMIZED = 2;

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT(int left, int top, int right, int bottom)
    {
        public int Left = left;
        public int Top = top;
        public int Right = right;
        public int Bottom = bottom;
    }

    // POINT structure required by WINDOWPLACEMENT structure
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT(int x, int y)
    {
        public int X = x;
        public int Y = y;
    }

    // WINDOWPLACEMENT stores the position, size, and state of a window
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public int showCmd;
        public POINT minPosition;
        public POINT maxPosition;
        public RECT normalPosition;
    }

    [DllImport("user32.dll")]
    private static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT windowPlacement);

    [DllImport("user32.dll")]
    private static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT windowPlacement);

    public static void SetPlacement(IntPtr windowHandle, string placementJson)
    {
        if (string.IsNullOrEmpty(placementJson))
        {
            return;
        }

        try
        {
            var placement = JsonConvert.DeserializeObject<WINDOWPLACEMENT>(placementJson);

            placement.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
            placement.flags = 0;
            placement.showCmd = placement.showCmd == SW_SHOWMINIMIZED ? SW_SHOWNORMAL : placement.showCmd;

            SetWindowPlacement(windowHandle, ref placement);
        }
        catch //(InvalidOperationException)
        {
            // Parsing placement XML failed. Fail silently.
        }
    }

    public static string GetPlacement(IntPtr windowHandle)
    {
        GetWindowPlacement(windowHandle, out var placement);

        return JsonConvert.SerializeObject(placement);
    }

    public static void SetPlacement(this Window window, string placementJson)
    {
        SetPlacement(new WindowInteropHelper(window).Handle, placementJson);
    }

    public static string GetPlacement(this Window window)
    {
        return GetPlacement(new WindowInteropHelper(window).Handle);
    }

    [DllImport("user32.dll")]
    static extern int GetWindowLong(IntPtr hwnd, int index);

    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

    [DllImport("user32.dll")]
    static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x,
        int y, int width, int height, uint flags);

    [DllImport("user32.dll")]
    static extern IntPtr SendMessage(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

    const int GWL_EXSTYLE = -20;
    const int WS_EX_DLGMODALFRAME = 0x0001;
    const int SWP_NOSIZE = 0x0001;
    const int SWP_NOMOVE = 0x0002;
    const int SWP_NOZORDER = 0x0004;
    const int SWP_FRAMECHANGED = 0x0020;
    const uint WM_SETICON = 0x0080;

    public static void RemoveIcon(this Window window)
    {
        var handle = new WindowInteropHelper(window).Handle;

        //SendMessage(handle, WM_SETICON, new IntPtr(1), IntPtr.Zero);
        //SendMessage(handle, WM_SETICON, IntPtr.Zero, IntPtr.Zero);

        SetWindowLong(handle, GWL_EXSTYLE, GetWindowLong(handle, GWL_EXSTYLE) & ~WS_EX_DLGMODALFRAME);
        SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
    }
}
