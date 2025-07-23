using Godot;
using System;
using System.Runtime.InteropServices;

public partial class WindowsMousePassthrough : Node
{
	private const int GWL_EXSTYLE = -20;
	private const uint WS_EX_APPWINDOW = 0x00040000;
	private const uint WS_EX_LAYERED = 0x00080000;
	private const uint WS_EX_TRANSPARENT = 0x00000020;
	private const uint WS_EX_TOOLWINDOW = 0x00000080;
	public override void _Ready()
	{
		if (OS.GetName() == "Windows")
		{
			var GlobalControls = GetNode<GodotObject>("/root/GlobalControlls");

			[DllImport("user32.dll")]
			static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
			[DllImport("user32.dll")]
			static extern int GetWindowLong(IntPtr hWnd, int nIndex);

			IntPtr _hWnd = (nint)DisplayServer.WindowGetNativeHandle(DisplayServer.HandleType.WindowHandle, (int)DisplayServer.MainWindowId);

			uint style = (uint)GetWindowLong(_hWnd, GWL_EXSTYLE);
			//SetWindowLong(_hWnd, GWL_EXSTYLE, ((style | WS_EX_TOOLWINDOW) & ~WS_EX_APPWINDOW) | WS_EX_LAYERED | WS_EX_TRANSPARENT); // Passthrough

			GlobalControls.Connect("gaming_mode_changed", Callable.From(() => { UpdateLong(); }));
			void UpdateLong()
			{
				uint new_style = style;
				if ((bool)GlobalControls.Get("gaming_mode_enabled"))
				{
					new_style = new_style | WS_EX_LAYERED | WS_EX_TRANSPARENT;
				}
				SetWindowLong(_hWnd, GWL_EXSTYLE, new_style);
			}
		}
	}
}
