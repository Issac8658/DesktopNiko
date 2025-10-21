using Godot;
using System;
using System.ComponentModel;

public partial class ValuesContainer : Node
{
	// Signals
	// Main
	[Signal] public delegate void ClickedEventHandler(int Difference);
	// TWM
	[Signal] public delegate void LanguageChangedEventHandler(byte Language);
	[Signal] public delegate void NikoAsWindowStateChangedEventHandler(bool ShowTaskbarIcon, bool NikoAlwaysOnTop);
	[Signal] public delegate void WindowsStateChangedEventHandler(bool AlwaysOnTop);
	[Signal] public delegate void ShutdownButtonHoveredEventHandler(bool Hovered);
	[Signal] public delegate void ShutdownPopupShowedEventHandler(bool Showed);
	[Signal] public delegate void ThemeChangedEventHandler(byte Theme);
	// Niko States
	[Signal] public delegate void NikoScaleChangedEventHandler(byte CurrentScale);
	[Signal] public delegate void FacepickChangedEventHandler(byte FacepickType, string Facepick);
	[Signal] public delegate void NikoHoverEventHandler(bool IsHovered);
	[Signal] public delegate void FacepickForcedEventHandler(string FacepickId);
	[Signal] public delegate void FacepickUnforcedEventHandler();
	[Signal] public delegate void NikoTimeToSleepChangedEventHandler(int TimeToSleep);
	[Signal] public delegate void GamingModeToggledEventHandler(bool GamingMode);
	[Signal] public delegate void NikoVisibilityChangedEventHandler(bool Visibility);

	// Duplicates
	// Main
	private ulong _clicks = 0;
	// TWM values
	private byte _language = 0; // 0 - eng, 1 - rus, 2 - deu, 3 - ukr
	private bool _niko_always_on_top = true;
	private bool _show_taskbar_icon = true;
	private bool _windows_on_top = true;
	private bool _shutdown_button_hovered = false;
	private bool _shutdown_popup_showed = false;
	private byte _current_theme = 0;
	// Niko States
	private byte _niko_scale = 1; // 0 - 0.5x, 1 - 1x, 2 - 2x, 3 - 3x, 4 - 4x
	private string _idle_facepick = "niko_smile";
	private string _speak_facepick = "niko_speak";
	private string _scare_facepick = "niko_shock";
	private string _scare_speak_facepick = "niko_surprised";
	private bool _niko_hovered = false;
	private bool _is_facepick_forced = false;
	private string _forced_facepick_id;
	private int _niko_time_to_sleep = 900; // in seconds
	private bool _gaming_mode_enabled = false;
	private bool _niko_visible = true;

	// Main
	public int TotalTime = 0;
	public ulong Clicks
	{
		get => _clicks;
		set
		{
			EmitSignal("Clicked", _clicks - value);
			_clicks = value;
		}
	}
	// TWM values
	public bool MouseSoundEnabled = true;
	public bool ShowSavingIcon = true;
	public bool ShowAchievements = true;
	public bool DoEvents = true;
	public byte Language
	{
		get => _language;
		set
		{
			_language = value;
			EmitSignal("LanguageChanged", value);
		}
	}
	public bool NikoAlwaysOnTop
	{
		get => _niko_always_on_top;
		set
		{
			_niko_always_on_top = value;
			EmitSignal("NikoAsWindowStateChanged", ShowTaskbarIcon, value);
		}
	}
	public bool ShowTaskbarIcon
	{
		get => _show_taskbar_icon;
		set
		{
			_show_taskbar_icon = value;
			EmitSignal("NikoAsWindowStateChanged", value, NikoAlwaysOnTop);
		}
	}
	public bool WindowsOnTop
	{
		get => _windows_on_top;
		set
		{
			_windows_on_top = value;
			EmitSignal("WindowsStateChanged", value);
		}
	}
	public bool IsShutdownButtonHovered
	{
		get => _shutdown_button_hovered;
		set
		{
			_shutdown_button_hovered = value;
			EmitSignal("ShutdownButtonHovered", value);
		}
	}
	public bool IsShutdownPupupShowed
	{
		get => _shutdown_popup_showed;
		set
		{
			_shutdown_popup_showed = value;
			EmitSignal("ShutdownPupupShowed", value);
		}
	}
	public byte CurrentTheme
	{
		get => _current_theme;
		set
		{
			_current_theme = value;
			EmitSignal("ThemeChanged", value);
		}
	}
	
	
	// Niko States
	public byte CurrentMeowSoundId = 0;
	public bool SnapToBottom = true;
	public bool PeacfulMode = false;

	//		Facepicks [
	public string IdleFacepick
	{
		set
		{
			_idle_facepick = value;
			EmitSignal("FacepickChanged", 0, value);
		}
		get => _idle_facepick;
	}
	public string SpeakFacepick
	{
		set
		{
			_speak_facepick = value;
			EmitSignal("FacepickChanged", 1, value);
		}
		get => _speak_facepick;
	}
	public string ScareFacepick
	{
		set
		{
			_scare_facepick = value;
			EmitSignal("FacepickChanged", 2, value);
		}
		get => _scare_facepick;
	}
	public string ScareSpeakFacepick
	{
		set
		{
			_scare_speak_facepick = value;
			EmitSignal("FacepickChanged", 3, value);
		}
		get => _scare_speak_facepick;
	}
	// 		] Facepicks
	public byte NikoScale // 0 - 0.5x, 1 - 1x, 2 - 2x, 3 - 3x, 4 - 4x
	{
		set
		{
			_niko_scale = value;
			EmitSignal("ScaleChanged", value);
		}
		get => _niko_scale;
	}
	public bool NikoHovered
	{
		set
		{
			_niko_hovered = value;
			EmitSignal("NikoHover", value);
		}
		get => _niko_hovered;
	}
	public bool IsFacepickForced
	{
		get => _is_facepick_forced;
		set{}
	}
	public string ForcedFacepickId
	{
		get => _forced_facepick_id;
		set{}
	}
	public int NikoTimeToSleep
	{
		get => _niko_time_to_sleep;
		set
		{
			_niko_time_to_sleep = value;
			EmitSignal("NikoTimeToSleepChanged", value);
		}
	}
	public bool GamingModeEnabled
	{
		get => _gaming_mode_enabled;
		set
		{
			_gaming_mode_enabled = value;
			EmitSignal("GamingModeToggled", value);
		}
	}
	public bool NikoVisible
	{
		get => _niko_visible;
		set
		{
			_niko_visible = value;
			EmitSignal("NikoVisibilityChanged", value);
		}
	}

	//Public functions
	public void ForceFacepick(string FacepickId)
	{
		_is_facepick_forced = true;
		_forced_facepick_id = FacepickId;
		EmitSignal("FacepickForced", FacepickId);
	}
	public void UnforceFacepick()
	{
		_is_facepick_forced = false;
		EmitSignal("FacepickUnforced");
	}
}
