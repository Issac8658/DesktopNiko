using Godot;
using Godot.NativeInterop;

public enum NikoState {Idle, Clicking, Sleepy, Sleeping, ForcedFacepic, InAnimation}

public partial class NikoController : Node
{
	public readonly float[] NikoScales = [0.5f, 1f, 2f, 3f, 4f];
	public readonly Godot.Collections.Dictionary<Key, float> Pitches = new()
	{
		// ocatave 4
		{Key.Z, -12f},
		{Key.S, -11f},
		{Key.X, -10f},
		{Key.D, -9f},
		{Key.C, -8f},
		{Key.V, -7f},
		{Key.G, -6f},
		{Key.B, -5f},
		{Key.H, -4f},
		{Key.N, -3f},
		{Key.J, -2f},
		{Key.M, -1f},

		// octave 5
		{Key.Comma,     0f},
		{Key.L,         1f},
		{Key.Period,    2f},
		{Key.Semicolon, 3f},
		{Key.Slash,     4f},

		{Key.Q,    0f},
		{Key.Key2, 1f},
		{Key.W,    2f},
		{Key.Key3, 3f},
		{Key.E,    4f},
		{Key.R,    5f},
		{Key.Key5, 6f},
		{Key.T,    7f},
		{Key.Key6, 8f},
		{Key.Y,    9f},
		{Key.Key7, 10f},
		{Key.U,    11f},
		
		// octave 6
		{Key.I,            12f},
		{Key.Key9,         13f},
		{Key.O,            14f},
		{Key.Key0,         15f},
		{Key.P,            16f},
		{Key.Bracketleft,  17f},
		{Key.Plus,         18f},
		{Key.Bracketright, 19f},
	};
	// im literally taked it from FL Studio
	public readonly Godot.Collections.Array<float> MeowPitches =
    [
        1.18f / 2f,
		1.63f / 2f,
		1.65f,
		1.55f / 2f,
		1.49f / 2f,
		1f,
		1.51f / 2f,
		1.1f
	];
	const int TIME_BEFORE_SLEEP = 6;

	private ValuesContainer _valuesContainer;
	private NikoSkinManager _skinManager;
	private AchievementsController _achievementsController;
	private Window _mainWindow;

	private double _clickTimer = 0;
	private bool _pressed = false;
	private uint _idleTime = 0;
	private NikoState _currentState = NikoState.Idle;

	[Export]
	public int CPS_ToScare = 8;

	[Export]
	public AudioStream[] Meows = [];
	[Export]
	public Control MenuPanel;
	[Export]
	public Button MenuButton;
	[Export]
	public Window WindowToOpen;
	[Export]
	public TextureRect NikoSpriteNode;
	[Export]
	public AnimationPlayer NikoAnimationPlayer;
	[Export]
	public AudioStreamPlayer MeowSoundPlayer;
	[Export]
	public GpuParticles2D SleepParticles;
	[Export]
	public ShaderMaterial WorldMachineMaterial;

	public override void _Ready()
	{
		OS.OpenMidiInputs();

		_valuesContainer = GetNode<ValuesContainer>("/root/ValuesContainer");
		_skinManager = GetNode<NikoSkinManager>("/root/NikoSkinManager");
		_achievementsController = GetNode<AchievementsController>("/root/AchievementsController");
		_mainWindow = GetWindow();

		_mainWindow.MouseEntered += () =>
		{
			MenuPanel.Modulate = Color.Color8(255, 255, 255, 255);
			_valuesContainer.NikoHovered = true;
		};
		_mainWindow.MouseExited += () =>
		{
			MenuPanel.Modulate = Color.Color8(255, 255, 255, 0);
			_valuesContainer.NikoHovered = false;
		};

		MenuButton.Pressed += () =>
		{
			WindowToOpen.Visible = !WindowToOpen.Visible;
		};

		NikoAnimationPlayer.AnimationFinished += animName => DoWhatNikoNeedToDo();
		NikoAnimationPlayer.AnimationStarted += animName => DoWhatNikoNeedToDo();
		_valuesContainer.WorldMachineToggled += Toggled => DoWhatNikoNeedToDo();
		_valuesContainer.FacepicForced += FacepicId => DoWhatNikoNeedToDo();
		_valuesContainer.FacepicUnforced += DoWhatNikoNeedToDo;
		_valuesContainer.GlobalTimerTicked += GlobalTimerTicked;
		_valuesContainer.NikoFlipped += Flipped => NikoSpriteNode.FlipH = Flipped;
		_skinManager.SkinChanged += SkinId => UpdateFacepic();

		NikoSpriteNode.GuiInput += Event =>
		{
			if (Event is InputEventMouseButton EventMouse)
				if (EventMouse.IsPressed() && EventMouse.ButtonIndex == MouseButton.Left && !_valuesContainer.IsFacepicForced)
					Click();
		};

		NikoSpriteNode.FlipH = _valuesContainer.NikoIsFlipped;
		DoWhatNikoNeedToDo();
	}

	public override void _Process(double delta)
	{
		if (_pressed)
			if (_clickTimer > 0)
			{
				_pressed = true;
				_clickTimer -= delta;
			}
			else
			{
				_pressed = false;
				DoWhatNikoNeedToDo();
			}
	}

    public override void _Input(InputEvent @event)
    {
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			if (Pitches.TryGetValue(keyEvent.Keycode, out float pitch))
			{
				AnimateClick();
				Meow(MeowPitches[_valuesContainer.CurrentMeowSoundId] * MidiToPitch(pitch + 60f));
			}
		}
		else if (@event is InputEventMidi MidiEvent && MidiEvent.Message == MidiMessage.NoteOn)
		{
			AnimateClick();
			Meow(MeowPitches[_valuesContainer.CurrentMeowSoundId] * MidiToPitch(MidiEvent.Pitch));
		}
    }



	private void UpdateFacepic()
	{
		SleepParticles.Emitting = false;
		switch (_currentState)
		{
			case NikoState.ForcedFacepic:
				SetSprite(_valuesContainer.ForcedFacepicId);
				break;

			case NikoState.Sleeping:
				SetSprite("sleep");
				SleepParticles.Emitting = true;
				if (!_achievementsController.IsAchievementTaked("sweet_dreams"))
				{
					_achievementsController.TakeAchievement("sweet_dreams");
				}
				break;

			case NikoState.Sleepy:
				SetSprite("sleepy");
				break;

			case NikoState.Idle:
				SetIdleFacepic();
				break;
		}
		NikoSpriteNode.Material = _valuesContainer.IsWorldMachine ? WorldMachineMaterial : null;
	}


	private void DoWhatNikoNeedToDo()
	{
		_valuesContainer.NikoScared = _valuesContainer.CPS > CPS_ToScare;

		if (NikoAnimationPlayer.IsPlaying())
			_currentState = NikoState.InAnimation;
		else if (_valuesContainer.IsFacepicForced)
			_currentState = NikoState.ForcedFacepic;
		else if (_idleTime >= _valuesContainer.NikoTimeToSleep)
			_currentState = NikoState.Sleeping;
		else if (_idleTime >= _valuesContainer.NikoTimeToSleep - TIME_BEFORE_SLEEP)
			_currentState = NikoState.Sleepy;
		else
			_currentState = NikoState.Idle;
		UpdateFacepic();
	}
	private void GlobalTimerTicked()
	{
		_idleTime += 1;
		DoWhatNikoNeedToDo();
	}

	private void Click()
	{
		if (!_valuesContainer.IsFacepicForced)
		{
			_valuesContainer.Clicks += 1;
			_idleTime = 0;
			Meow();
			AnimateClick();
		
			if (_valuesContainer.Clicks >= 10 && !_achievementsController.IsAchievementTaked("ten_clicks"))
				_achievementsController.TakeAchievement("ten_clicks");
			if (_valuesContainer.Clicks >= 100 && !_achievementsController.IsAchievementTaked("one_hundred_clicks"))
				_achievementsController.TakeAchievement("one_hundred_clicks");
			if (_valuesContainer.Clicks >= 1000 && !_achievementsController.IsAchievementTaked("one_thousand_clicks"))
				_achievementsController.TakeAchievement("one_thousand_clicks");
			if (_valuesContainer.Clicks >= 100000 && !_achievementsController.IsAchievementTaked("one_hundred_thousand_clicks"))
				_achievementsController.TakeAchievement("one_hundred_thousand_clicks");
			if (_valuesContainer.Clicks >= 1000000 && !_achievementsController.IsAchievementTaked("one_million_clicks"))
				_achievementsController.TakeAchievement("one_million_clicks");
			if (_valuesContainer.Clicks >= 1000000000 && !_achievementsController.IsAchievementTaked("one_billion_clicks"))
				_achievementsController.TakeAchievement("one_billion_clicks");
		}
	}

	private void Meow(float pitch = 1f)
	{
		if (MeowSoundPlayer.Stream != Meows[_valuesContainer.CurrentMeowSoundId])
			MeowSoundPlayer.Stream = Meows[_valuesContainer.CurrentMeowSoundId];
		MeowSoundPlayer.PitchScale = pitch;
		MeowSoundPlayer.Play();
	}
	
	private void AnimateClick()
	{
		NikoAnimationPlayer.Stop();
		NikoAnimationPlayer.Play("click");
	}

	public void SetSprite(string spriteId)
	{
		float scale = NikoScales[_valuesContainer.NikoScale];
		Texture2D sprite = _skinManager.GetCurrentSkinSprite(spriteId);
		NikoSpriteNode.Texture = sprite;
		NikoSpriteNode.CustomMinimumSize = sprite.GetSize() * scale * _skinManager.GetCurrentSkinBaseScale();
	}

	// for animations
	public void SetSpeakFacepic()
	{
		if (_valuesContainer.NikoScared && !_valuesContainer.PeacfulMode)
			SetSprite(_valuesContainer.ScaredSpeakFacepic);
		else
			SetSprite(_valuesContainer.SpeakFacepic);
	}
	public void SetIdleFacepic()
	{
		if (_valuesContainer.IsFacepicForced)
			SetSprite(_valuesContainer.ForcedFacepicId);
		if (_valuesContainer.NikoScared && !_valuesContainer.PeacfulMode)
			SetSprite(_valuesContainer.ScaredFacepic);
		else
			SetSprite(_valuesContainer.IdleFacepic);
	}

	public static float MidiToPitch(float Midi)
	{
		return Mathf.Pow(2, (Midi - 60) / 12);
	}
}
