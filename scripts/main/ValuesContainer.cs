using System.Collections.Generic;
using Godot;

public partial class ValuesContainer : Node
{
	public enum FacepicType
	{
		Default = 0,
		Speak = 1,
		Scared = 2,
		ScaredSpeak = 3
	}

// idk what is this :3
	private Timer GlobalTimer = new();
// Signals
	// TWM
	[Signal] public delegate void LanguageChangedEventHandler(byte Language);
	[Signal] public delegate void NikoAsWindowStateChangedEventHandler(bool ShowTaskbarIcon, bool NikoAlwaysOnTop);
	[Signal] public delegate void WindowsStateChangedEventHandler(bool AlwaysOnTop);
	[Signal] public delegate void EventDonedEventHandler();
	//[Signal] public delegate void ShutdownButtonHoveredEventHandler(bool Hovered);
	//[Signal] public delegate void ShutdownPopupShowedEventHandler(bool Showed);
	[Signal] public delegate void ThemeChangedEventHandler(byte Theme);
	// Niko States
	[Signal] public delegate void NikoScaleChangedEventHandler(byte CurrentScale);
	[Signal] public delegate void FacepicChangedEventHandler(FacepicType FacepicType, string Facepic);
	[Signal] public delegate void NikoTimeToSleepChangedEventHandler(int TimeToSleep);
	[Signal] public delegate void GamingModeToggledEventHandler(bool GamingMode);
	[Signal] public delegate void NikoSkinChangedEventHandler(string SkinId);
	[Signal] public delegate void WorldMachineToggledEventHandler(bool Toggled);
	[Signal] public delegate void NikoFlippedEventHandler(bool Flipped);

	#region Variables to Save





    #region >Duplicates





    #region >>Main
	private ulong _clicks = 0;
	private string _verison;
	#endregion


    #region >>TWM
	private byte _language = 0; // 0 - eng, 1 - rus, 2 - deu, 3 - ukr
	private bool _nikoAlwaysOnTop = true;
	private bool _showTaskbarIcon = true;
	private bool _windowsAlwaysOnTop = true;
	private int _donedEvents = 0;
	//private bool _shutdownButtonHovered = false;
	//private bool _shutdownPopupShowed = false;
	private Color _themeColorMain;
	private Color _themeColorHover;
	private Color _themeColorOutlineHover;
	private Color _themeColorBaseHover;
	private Color _themeColorOutlinePressed;
	private Color _themeColorBasePressed;
	private Color _themeColorBackground;
	private bool _themeRainbow;
	#endregion
	#region >>Niko States
	private byte _nikoScale = 1; // 0 - 0.5x, 1 - 1x, 2 - 2x, 3 - 3x, 4 - 4x
	private string _idleFacepic = "smile";
	private string _speakFacepic = "speak";
	private string _scaredFacepic = "shock";
	private string _scaredSpeakFacepic = "surprised";
	private uint _nikoTimeToSleep = 900; // in seconds
	private bool _gamingModeEnabled = false;
	private string _currentSkin = ""; // sets by SaveLoad.cs
	private bool _isWorldMachine = false;
	private bool _nikoIsFlipped = false;
	#endregion
	#endregion

	#region >Main
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
	#endregion
	#region >TWM
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
			switch (value)
			{
				case 0:
					{
						TranslationServer.SetLocale("en");
						break;
					}
				case 1:
					{
						TranslationServer.SetLocale("ru");
						break;
					}
				case 2:
					{
						TranslationServer.SetLocale("de");
						break;
					}
				case 3:
					{
						TranslationServer.SetLocale("ua");
						break;
					}
			}
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
			EmitSignal("EventDoned");
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
	public Color ThemeColorMain{
		get => _themeColorMain;
		set
		{
			_themeColorMain = value;
			RenderingServer.GlobalShaderParameterSet("theme_color_main", value);
		}
	}
	public Color ThemeColorHover{
		get => _themeColorHover;
		set
		{
			_themeColorHover = value;
			RenderingServer.GlobalShaderParameterSet("theme_color_hover", value);
		}
	}
	public Color ThemeColorOutlineHover{
		get => _themeColorOutlineHover;
		set
		{
			_themeColorOutlineHover = value;
			RenderingServer.GlobalShaderParameterSet("theme_color_button_outline_hover", value);
		}
	}
	public Color ThemeColorBaseHover{
		get => _themeColorBaseHover;
		set
		{
			_themeColorBaseHover = value;
			RenderingServer.GlobalShaderParameterSet("theme_color_button_base_hover", value);
		}
	}
	public Color ThemeColorOutlinePressed{
		get => _themeColorOutlinePressed;
		set
		{
			_themeColorOutlinePressed = value;
			RenderingServer.GlobalShaderParameterSet("theme_color_button_outline_pressed", value);
		}
	}
	public Color ThemeColorBasePressed{
		get => _themeColorBasePressed;
		set
		{
			_themeColorBasePressed = value;
			RenderingServer.GlobalShaderParameterSet("theme_color_button_base_pressed", value);
		}
	}
	public Color ThemeColorBackground{
		get => _themeColorBackground;
		set
		{
			_themeColorBackground = value;
			RenderingServer.GlobalShaderParameterSet("theme_color_background", value);
		}
	}
	public bool ThemeRainbow{
		get => _themeRainbow;
		set
		{
			_themeRainbow = value;
			RenderingServer.GlobalShaderParameterSet("theme_rainbow", value);
		}
	}
	#endregion
	
	#region >NikoStates
	public byte CurrentMeowSoundId = 0;
	public bool SnapToBottom = true;
	public bool PeacfulMode = false;

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
	public string ScaredFacepic
	{
		set
		{
			_scaredFacepic = value;
			EmitSignal("FacepicChanged", 2, value);
		}
		get => _scaredFacepic;
	}
	public string ScaredSpeakFacepic
	{
		set
		{
			_scaredSpeakFacepic = value;
			EmitSignal("FacepicChanged", 3, value);
		}
		get => _scaredSpeakFacepic;
	}
	public byte NikoScale // 0 - 0.5x, 1 - 1x, 2 - 2x, 3 - 3x, 4 - 4x
	{
		set
		{
			_nikoScale = value;
			EmitSignal("NikoScaleChanged", value);
		}
		get => _nikoScale;
	}
	public uint NikoTimeToSleep
	{
		get => _nikoTimeToSleep;
		set
		{
			_nikoTimeToSleep = value;
			EmitSignal("NikoTimeToSleepChanged", value);
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
	public bool IsWorldMachine
	{
		set
		{
			_isWorldMachine = value;
			EmitSignal("WorldMachineToggled", value);
		}
		get => _isWorldMachine;
	}
	public bool NikoIsFlipped
	{
		set
		{
			_nikoIsFlipped = value;
			EmitSignal("NikoFlipped", value);
		}
		get => _nikoIsFlipped;
	}
	#endregion
	#endregion
	#region Other
// Signals
	[Signal] public delegate void ClickedEventHandler(int Difference);
	[Signal] public delegate void GlobalTimerTickedEventHandler();
	// Niko States
	[Signal] public delegate void NikoHoverEventHandler(bool IsHovered);
	[Signal] public delegate void FacepicForcedEventHandler(string FacepicId);
	[Signal] public delegate void FacepicUnforcedEventHandler();
	[Signal] public delegate void NikoVisibilityChangedEventHandler(bool Visibility);

	private Queue<double> _CPSWindow = [];
	public int CPS { get => _CPSWindow.Count; }
// Duplicates
	// Main
	private uint _globalTime = 0; // Time for current session
	// Niko States
	private bool _nikoHovered = false;
	private bool _nikoVisible = true;
	private bool _isFacepicForced = false;
	private string _forcedFacepicId;

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
		get => _isFacepicForced;
		set{}
	}
	public string ForcedFacepicId
	{
		get => _forcedFacepicId;
		set
		{
			ForceFacepic(value);
		}
	}
//Public functions
	public void ForceFacepic(string FacepicId)
	{
		_isFacepicForced = true;
		_forcedFacepicId = FacepicId;
		EmitSignal("FacepicForced", FacepicId);
	}
	public void UnforceFacepic()
	{
		_isFacepicForced = false;
		EmitSignal("FacepicUnforced");
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
		Clicked += diff => {
			if (diff == 1)
			{
    			double CurrentTime = Time.GetTicksMsec() / 1000.0;

    			_CPSWindow.Enqueue(CurrentTime);
    			while (_CPSWindow.Count > 0 && CurrentTime - _CPSWindow.Peek() > 1.0)
    			    _CPSWindow.Dequeue();
			}
		};
		
		AddChild(GlobalTimer);
		GlobalTimer.Start();
	}
    public override void _Process(double delta)
    {
    	double CurrentTime = Time.GetTicksMsec() / 1000.0;
    	if (_CPSWindow.Count > 0 && CurrentTime - _CPSWindow.Peek() > 1.0)
    	    _CPSWindow.Dequeue();
    }

	#endregion
}
