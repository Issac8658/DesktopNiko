using Godot;
using System;
using System.ComponentModel;

public partial class ValuesContainer : Node
{
	private Timer GlobalTimer = new();
	// Signals
	// Main
	[Signal] public delegate void ClickedEventHandler(int Difference);
	// TWM
	[Signal] public delegate void LanguageChangedEventHandler(byte Language);
	[Signal] public delegate void NikoAsWindowStateChangedEventHandler(bool ShowTaskbarIcon, bool NikoAlwaysOnTop);
	[Signal] public delegate void WindowsStateChangedEventHandler(bool AlwaysOnTop);
	//[Signal] public delegate void ShutdownButtonHoveredEventHandler(bool Hovered);
	//[Signal] public delegate void ShutdownPopupShowedEventHandler(bool Showed);
	[Signal] public delegate void ThemeChangedEventHandler(byte Theme);
	// Niko States
	[Signal] public delegate void NikoScaleChangedEventHandler(byte CurrentScale);
	[Signal] public delegate void FacepicChangedEventHandler(byte FacepickType, string Facepick);
	[Signal] public delegate void NikoHoverEventHandler(bool IsHovered);
	[Signal] public delegate void FacepickForcedEventHandler(string FacepickId);
	[Signal] public delegate void FacepickUnforcedEventHandler();
	[Signal] public delegate void NikoTimeToSleepChangedEventHandler(int TimeToSleep);
	[Signal] public delegate void GamingModeToggledEventHandler(bool GamingMode);
	[Signal] public delegate void NikoVisibilityChangedEventHandler(bool Visibility);
	[Signal] public delegate void NikoSkinChangedEventHandler(string SkinId);
	[Signal] public delegate void GlobalTimerTickedEventHandler();

	// Duplicates
	// Main
	private ulong _clicks = 0;
	public uint _totalTime = 0; // Time for all sessions
	private uint _globalTime = 0; // Time for current session
	// TWM values
	private byte _language = 0; // 0 - eng, 1 - rus, 2 - deu, 3 - ukr
	private bool _nikoAlwaysOnTop = true;
	private bool _showTaskbarIcon = true;
	private bool _windowsOnTop = true;
	//private bool _shutdownButtonHovered = false;
	//private bool _shutdownPopupShowed = false;
	private byte _currentTheme = 0;
	// Niko States
	private byte _nikoScale = 1; // 0 - 0.5x, 1 - 1x, 2 - 2x, 3 - 3x, 4 - 4x
	private string _idleFacepic = "smile";
	private string _speakFacepic = "speak";
	private string _scareFacepic = "shock";
	private string _scareSpeakFacepic = "surprised";
	private bool _nikoHovered = false;
	private bool _isFacepickForced = false;
	private string _forcedFacepickId;
	private int _nikoTimeToSleep = 900; // in seconds
	private bool _gamingModeEnabled = false;
	private bool _nikoVisible = true;
	private string _currentSkin = "";

	// Main
	public uint TotalTime{ // Time for all sessions
		set {}
		get => _totalTime;
	}
	public uint GlobalTime{ // Time for current session
		set {}
		get => _globalTime;
	}
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
		get => _nikoAlwaysOnTop;
		set
		{
			_nikoAlwaysOnTop = value;
			EmitSignal("NikoAsWindowStateChanged", ShowTaskbarIcon, value);
		}
	}
	public bool ShowTaskbarIcon
	{
		get => _showTaskbarIcon;
		set
		{
			_showTaskbarIcon = value;
			EmitSignal("NikoAsWindowStateChanged", value, NikoAlwaysOnTop);
		}
	}
	public bool WindowsOnTop
	{
		get => _windowsOnTop;
		set
		{
			_windowsOnTop = value;
			EmitSignal("WindowsStateChanged", value);
		}
	}
	//public bool IsShutdownButtonHovered
	//{
	//	get => _shutdownButtonHovered;
	//	set
	//	{
	//		_shutdownButtonHovered = value;
	//		EmitSignal("ShutdownButtonHovered", value);
	//	}
	//}
	//public bool IsShutdownPupupShowed
	//{
	//	get => _shutdownPopupShowed;
	//	set
	//	{
	//		_shutdownPopupShowed = value;
	//		EmitSignal("ShutdownPupupShowed", value);
	//	}
	//}
	public byte CurrentTheme
	{
		get => _currentTheme;
		set
		{
			_currentTheme = value;
			EmitSignal("ThemeChanged", value);
		}
	}
	
	
	// Niko States
	public byte CurrentMeowSoundId = 0;
	public bool SnapToBottom = true;
	public bool PeacfulMode = false;
	public bool NikoScared = false;

	//		Facepicks [
	public string IdleFacepic
	{
		set
		{
			_idleFacepic = value;
			EmitSignal("FacepicChanged", 0, value);
		}
		get => _idleFacepic;
	}
	public string SpeakFacepic
	{
		set
		{
			_speakFacepic = value;
			EmitSignal("FacepicChanged", 1, value);
		}
		get => _speakFacepic;
	}
	public string ScareFacepic
	{
		set
		{
			_scareFacepic = value;
			EmitSignal("FacepicChanged", 2, value);
		}
		get => _scareFacepic;
	}
	public string ScareSpeakFacepic
	{
		set
		{
			_scareSpeakFacepic = value;
			EmitSignal("FacepicChanged", 3, value);
		}
		get => _scareSpeakFacepic;
	}
	// 		] Facepicks
	public byte NikoScale // 0 - 0.5x, 1 - 1x, 2 - 2x, 3 - 3x, 4 - 4x
	{
		set
		{
			_nikoScale = value;
			EmitSignal("ScaleChanged", value);
		}
		get => _nikoScale;
	}
	public bool NikoHovered
	{
		set
		{
			_nikoHovered = value;
			EmitSignal("NikoHover", value);
		}
		get => _nikoHovered;
	}
	public bool IsFacepicForced
	{
		get => _isFacepickForced;
		set{}
	}
	public string ForcedFacepicId
	{
		get => _forcedFacepickId;
		set{}
	}
	public int NikoTimeToSleep
	{
		get => _nikoTimeToSleep;
		set
		{
			_nikoTimeToSleep = value;
			EmitSignal("NikoTimeToSleepChanged", value);
		}
	}
	public bool GamingModeEnabled
	{
		get => _gamingModeEnabled;
		set
		{
			_gamingModeEnabled = value;
			EmitSignal("GamingModeToggled", value);
		}
	}
	public bool NikoVisible
	{
		get => _nikoVisible;
		set
		{
			_nikoVisible = value;
			EmitSignal("NikoVisibilityChanged", value);
		}
	}
	public string CurrentSkin
	{
		set
		{
			_currentSkin = value;
			EmitSignal("NikoSkinChanged", value);
		}
		get => _currentSkin;
	}

	//Public functions
	public void ForceFacepick(string FacepickId)
	{
		_isFacepickForced = true;
		_forcedFacepickId = FacepickId;
		EmitSignal("FacepickForced", FacepickId);
	}
	public void UnforceFacepick()
	{
		_isFacepickForced = false;
		EmitSignal("FacepickUnforced");
	}

	public override void _Ready()
	{
		GlobalTimer.WaitTime = 1;
		GlobalTimer.OneShot = false;

		GlobalTimer.Timeout += () =>
		{
			_globalTime += 1;
			_totalTime += 1;
			EmitSignal("GlobalTimerTicked");
		};
		
		AddChild(GlobalTimer);
		GlobalTimer.Start();
	}
}
