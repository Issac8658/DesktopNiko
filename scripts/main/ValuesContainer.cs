using Godot;

public partial class ValuesContainer : Node
{
// idk what this :3
	private Timer GlobalTimer = new();
// Signals
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
	[Signal] public delegate void NikoTimeToSleepChangedEventHandler(int TimeToSleep);
	[Signal] public delegate void GamingModeToggledEventHandler(bool GamingMode);
	[Signal] public delegate void NikoSkinChangedEventHandler(string SkinId);

// Duplicates
	// Main
	private ulong _clicks = 0;
	private string _verison;

	// TWM values
	private byte _language = 0; // 0 - eng, 1 - rus, 2 - deu, 3 - ukr
	private bool _nikoAlwaysOnTop = true;
	private bool _showTaskbarIcon = true;
	private bool _windowsAlwaysOnTop = true;
	private int _donedEvents = 0;
	//private bool _shutdownButtonHovered = false;
	//private bool _shutdownPopupShowed = false;
	private byte _currentTheme = 0;

	// Niko States
	private byte _nikoScale = 1; // 0 - 0.5x, 1 - 1x, 2 - 2x, 3 - 3x, 4 - 4x
	private string _idleFacepic = "smile";
	private string _speakFacepic = "speak";
	private string _scareFacepic = "shock";
	private string _scareSpeakFacepic = "surprised";
	private int _nikoTimeToSleep = 900; // in seconds
	private bool _gamingModeEnabled = false;
	private string _currentSkin = "";

// Publics
	// Main
	public uint TotalTime = 0;
	public ulong Clicks
	{
		get => _clicks;
		set
		{
			EmitSignal("Clicked", value - _clicks);
			_clicks = value;
		}
	}
	public string Version
	{
		get => _verison;
		set {}
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
	public bool WindowsAlwaysOnTop
	{
		get => _windowsAlwaysOnTop;
		set
		{
			_windowsAlwaysOnTop = value;
			EmitSignal("WindowsStateChanged", value);
		}
	}
	public int DonedEvents
	{
		get => _donedEvents;
		set
		{
			_donedEvents = value;
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
			EmitSignal("NikoScaleChanged", value);
		}
		get => _nikoScale;
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
	[Export]
	public string CurrentSkin
	{
		set
		{
			_currentSkin = value;
			EmitSignal("NikoSkinChanged", value);
		}
		get => _currentSkin;
	}







	//variables that do not need to be saved
// Signals
	// Main
	[Signal] public delegate void ClickedEventHandler(int Difference);
	[Signal] public delegate void GlobalTimerTickedEventHandler();
	// Niko States
	[Signal] public delegate void NikoHoverEventHandler(bool IsHovered);
	[Signal] public delegate void FacepickForcedEventHandler(string FacepickId);
	[Signal] public delegate void FacepickUnforcedEventHandler();
	[Signal] public delegate void NikoVisibilityChangedEventHandler(bool Visibility);

// Duplicates
	// Main
	private uint _globalTime = 0; // Time for current session
	// Niko States
	private bool _nikoHovered = false;
	private bool _nikoVisible = true;
	private bool _isFacepickForced = false;
	private string _forcedFacepickId;

// Publics
	// Main
	public uint GlobalTime{ // Time for current session
		set {}
		get => _globalTime;
	}

	// Niko States
	public bool NikoScared = false;
	public bool NikoHovered
	{
		set
		{
			_nikoHovered = value;
			EmitSignal("NikoHover", value);
		}
		get => _nikoHovered;
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
	public bool IsFacepicForced
	{
		get => _isFacepickForced;
		set{}
	}
	public string ForcedFacepicId
	{
		get => _forcedFacepickId;
		set
		{
			ForceFacepick(value);
		}
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
		_verison = (string)ProjectSettings.GetSetting("application/config/version");
		GlobalTimer.WaitTime = 1;
		GlobalTimer.OneShot = false;

		GlobalTimer.Timeout += () =>
		{
			_globalTime += 1;
			TotalTime += 1;
			EmitSignal("GlobalTimerTicked");
		};
		
		AddChild(GlobalTimer);
		GlobalTimer.Start();
	}
}
