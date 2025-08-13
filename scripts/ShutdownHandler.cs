using Godot;

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

public sealed partial class ShutdownHandler : Node
{
	// some signals
	[Signal]
	public delegate void OnShutdownSignalEventHandler();
	[Signal]
	public delegate void OnShutdownAbortSignalEventHandler();

	// public stuff
	public bool IsShuttingDown { get; private set; } = false;

	// private stuff
	private IntPtr _hWnd = IntPtr.Zero;
	private IntPtr _originalWndProc = IntPtr.Zero;

	/*
	 * i put this stuff there, so garbage collector wont clean it accidentally,
	 * so program wont crash again. whatever, im calling GC.KeepAlive, so i don't
	 * think it would crash again, i hope.
	 */
	private WindowProcedure _wndProcDelegate = null;

	public override void _Ready()
	{
		if (OS.GetName() != "Windows")
			return;

		_wndProcDelegate = new WindowProcedure(_WndProc);

		_hWnd = (IntPtr)DisplayServer.WindowGetNativeHandle(DisplayServer.HandleType.WindowHandle, 0);
		IntPtr newWndProc = Marshal.GetFunctionPointerForDelegate(_wndProcDelegate);

		_originalWndProc = NativeMethods.SetWindowLong(_hWnd, GWLP_WNDPROC, newWndProc);
		if (_originalWndProc == IntPtr.Zero)
			throw new Win32Exception(Marshal.GetLastWin32Error());
			
		GC.KeepAlive(_wndProcDelegate); // forcing garbage collector to not clean it again, as it did previously.
	}

	[Obsolete("Use IsShuttingDown property instead.")]
	public bool IsWindowsShuttingDown()
		=> IsShuttingDown;

	private IntPtr _WndProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
	{
		switch (uMsg)
		{
			case WM_QUERYENDSESSION:
				IsShuttingDown = true;

				NativeMethods.ShutdownBlockReasonCreate(_hWnd, TranslationServer.Translate("SHUTDOWN_BLOCK_MESSAGE"));

				EmitSignal(SignalName.OnShutdownSignal);
				return IntPtr.Zero; // FALSE
			case WM_ENDSESSION:
				IsShuttingDown = wParam != IntPtr.Zero;

				if (!IsShuttingDown)
				{
					EmitSignal(SignalName.OnShutdownAbortSignal);
					NativeMethods.ShutdownBlockReasonDestroy(_hWnd);
				}
				break;
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
