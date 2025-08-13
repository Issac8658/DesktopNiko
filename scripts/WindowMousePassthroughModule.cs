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
	private const uint DEFAULT_WS_STYLE = 0x20000810;
	
	private const int SW_SHOW = 5;
	private const int SW_HIDE = 0;

	public void UpdateWindowsExStyles(Window window, bool NoPanelIcon)
	{
		if (OS.GetName() == "Windows")
		{
			[DllImport("user32.dll")]
			static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
			[DllImport("user32.dll")]
			static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
			//[DllImport("user32.dll")]
			//static extern int GetWindowLong(IntPtr hWnd, int nIndex);

			IntPtr hWnd = (nint)DisplayServer.WindowGetNativeHandle(DisplayServer.HandleType.WindowHandle, window.GetWindowId());

			uint style = DEFAULT_WS_STYLE;
			if ((bool)window.Get("always_on_top"))
			{
				style |= WS_EX_TOPMOST;
			}
			if ((bool)window.Get("mouse_passthrough"))
			{
				style |= WS_EX_LAYERED | WS_EX_TRANSPARENT;
			}
			if (NoPanelIcon)
			{
				style = (style & ~WS_EX_APPWINDOW) | WS_EX_TOOLWINDOW;
			}
			ShowWindow(hWnd, SW_HIDE);
			SetWindowLong(hWnd, GWL_EXSTYLE, style);
			ShowWindow(hWnd, SW_SHOW);
		}
	}
}
