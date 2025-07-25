using Godot;

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

public sealed partial class ShutdownHandler : Node
{
	// exported stuff
	[Export]
	public bool ShouldPreventShutdown = true;
	
	// some signals
	[Signal]
	public delegate void OnShutdownSignalEventHandler();
	[Signal]
	public delegate void OnShutdownAbortSignalEventHandler();
	
	// private stuff
	private IntPtr _hWnd = IntPtr.Zero;
	private IntPtr _originalWndProc = IntPtr.Zero;
	private bool _isWindowsShuttingDown = false;
	
	public override void _Ready()
	{
		if (OS.GetName() != "Windows")
			return;
		
		_hWnd = (IntPtr)DisplayServer.WindowGetNativeHandle(DisplayServer.HandleType.WindowHandle, 0);
		IntPtr newWndProc = Marshal.GetFunctionPointerForDelegate(new WindowProcedure(_WndProc));

		_originalWndProc = NativeMethods.SetWindowLong(_hWnd, GWLP_WNDPROC, newWndProc);
		if (_originalWndProc == IntPtr.Zero)
			throw new Win32Exception(Marshal.GetLastWin32Error());
	}
	
	public bool IsWindowsShuttingDown()
	{
		if (OS.GetName() != "Windows")
			return false;
		
		return _isWindowsShuttingDown;
	}
	
	
	private void _HandleShutdown(){
		EmitSignal(SignalName.OnShutdownSignal);}
	
	private void _HandleShutdownAbort()
		=> EmitSignal(SignalName.OnShutdownAbortSignal);
	
	private IntPtr _WndProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
	{
		if (ShouldPreventShutdown)
		{
			switch (uMsg)
			{
				case WM_QUERYENDSESSION:
					_isWindowsShuttingDown = true;
					
					NativeMethods.ShutdownBlockReasonCreate(_hWnd, TranslationServer.Translate("SHUTDOWN_BLOCK_MESSAGE"));
					
					CallDeferred(nameof(_HandleShutdown));
					return IntPtr.Zero; // FALSE
				case WM_ENDSESSION:
					_isWindowsShuttingDown = wParam != IntPtr.Zero;
					
					if (_isWindowsShuttingDown)
					{
						CallDeferred(nameof(_HandleShutdownAbort));
						NativeMethods.ShutdownBlockReasonDestroy(_hWnd);
					}
					break;
			}
		}

		return NativeMethods.CallWindowProc(_originalWndProc, hWnd, uMsg, wParam, lParam);
	}
	
	// some winapi consts.
	private const uint WM_QUERYENDSESSION = 0x0011;
	private const uint WM_ENDSESSION = 0x0016;
		
	private const int GWLP_WNDPROC = -4;

	// some dll imports
	private static class NativeMethods
	{
		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern bool ShutdownBlockReasonCreate(IntPtr hWnd, string pwszReason);

		[DllImport("user32.dll")]
		public static extern bool ShutdownBlockReasonDestroy(IntPtr hWnd);
		
		[DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
		private static extern IntPtr _SetWindowLong64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
		private static extern IntPtr _SetWindowLong32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll", EntryPoint = "CallWindowProcA", SetLastError = true)]
		public static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint uMsg,
			IntPtr wParam, IntPtr lParam);

		/*
		  shit that calls _SetWindowLong32 or _SetWindowLong64 depending on os arch
		  bc windows is trash. fuck windows.
		*/
		public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
		{
			if (IntPtr.Size == 4)
				return _SetWindowLong32(hWnd, nIndex, dwNewLong);

			return _SetWindowLong64(hWnd, nIndex, dwNewLong);
		}
	}
	
	// just a delegate for wndproc
	private delegate IntPtr WindowProcedure(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
}
