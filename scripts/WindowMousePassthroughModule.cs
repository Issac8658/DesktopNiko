using Godot;
using System;
using System.Runtime.InteropServices;

public partial class WindowMousePassthroughModule : Node
{
	private const int GWL_EXSTYLE = -20;
	private const uint WS_EX_APPWINDOW = 0x00040000;
	private const uint WS_EX_LAYERED = 0x00080000;
	private const uint WS_EX_TRANSPARENT = 0x00000020;
	private const uint WS_EX_TOOLWINDOW = 0x00000080;
	private const uint WS_EX_TOPMOST = 0x00000008;
	private const uint DEFAULT_WS_STYLE = 0x20000900;
	private const uint LWA_ALPHA = 0x2;
	
	private const int SW_SHOW = 5;
	private const int SW_HIDE = 0;

	public void UpdateWindowsExStyles(Window window, bool HideTaskbarIcon, bool ForceWindowUpdate = true)
	{
		if (OS.GetName() == "Windows")
		{
			[DllImport("user32.dll")]
			static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
			[DllImport("user32.dll")]
			static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
			[DllImport("user32.dll")]
			static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
			//[DllImport("user32.dll")]
			//static extern int GetWindowLong(IntPtr hWnd, int nIndex);

			IntPtr hWnd = (nint)DisplayServer.WindowGetNativeHandle(DisplayServer.HandleType.WindowHandle, window.GetWindowId());

			uint style = DEFAULT_WS_STYLE;
			if (window.MousePassthrough)
			{
				style |= WS_EX_LAYERED; //проподает из-за этого
				style |= WS_EX_TRANSPARENT;
			}
			if (window.AlwaysOnTop)
			{
				style |= WS_EX_TOPMOST;
			}
			if (HideTaskbarIcon)
			{
				style = (style & ~WS_EX_APPWINDOW) | WS_EX_TOOLWINDOW;
			}
			if (ForceWindowUpdate)
			_ = SetWindowLong(hWnd, GWL_EXSTYLE, style);
			SetLayeredWindowAttributes(hWnd, 0, 255, LWA_ALPHA);
			if (ForceWindowUpdate)
			ShowWindow(hWnd, SW_SHOW);
		}
	}
}
