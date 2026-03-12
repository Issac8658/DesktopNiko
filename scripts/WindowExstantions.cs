using Godot;
using System;
using System.Runtime.InteropServices;

public partial class WindowExstantions : Node
{
	private const int GWL_EXSTYLE = -20;
	private const uint WS_EX_APPWINDOW = 0x00040000;
	private const uint WS_EX_LAYERED = 0x00080000;
	private const uint WS_EX_TRANSPARENT = 0x00000020;
	private const uint WS_EX_TOOLWINDOW = 0x00000080;
	private const uint WS_EX_TOPMOST = 0x00000008;
	private const uint DEFAULT_WS_STYLE = 0x20000900;
	private const uint LWA_ALPHA = 0x2;

	const uint SWP_FRAMECHANGED = 0x0020;
	const uint SWP_NOMOVE = 0x0002;
	const uint SWP_NOSIZE = 0x0001;
	const uint SWP_NOZORDER = 0x0004;
	
	private const int SW_SHOW = 5;
	private const int SW_HIDE = 0;

	public void UpdateWindowsExStyles(Window window, bool HideTaskbarIcon, bool ForceWindowUpdate = false)
	{
		if (OS.GetName() == "Windows")
		{
			[DllImport("user32.dll")]
			static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
			[DllImport("user32.dll")]
			static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
			[DllImport("user32.dll")]
			static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
			[DllImport("user32.dll")]
			static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
			//[DllImport("user32.dll")]
			//static extern int GetWindowLong(IntPtr hWnd, int nIndex);

			IntPtr hwnd = (nint)DisplayServer.WindowGetNativeHandle(DisplayServer.HandleType.WindowHandle, window.GetWindowId());

			uint style = DEFAULT_WS_STYLE;
			if (window.AlwaysOnTop)
			{
				style |= WS_EX_TOPMOST;
			}
			if (HideTaskbarIcon)
			{
				style = (style & ~WS_EX_APPWINDOW) | WS_EX_TOOLWINDOW;
			}
			if (window.MousePassthrough)
			{
				style |= WS_EX_LAYERED;
				style |= WS_EX_TRANSPARENT;
			}
			if (ForceWindowUpdate)
				ShowWindow(hwnd, SW_HIDE);
			_ = SetWindowLong(hwnd, GWL_EXSTYLE, style);
			SetLayeredWindowAttributes(hwnd, 0, 255, LWA_ALPHA);
			if (ForceWindowUpdate)
			{
				SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_FRAMECHANGED | SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER);
				ShowWindow(hwnd, SW_SHOW);
			}
		}
	}
}
