using Godot;

public enum NikoState {Idle, Clicking, Sleepy, Sleeping, ForcedFacepic, InAnimation}

public partial class NikoController : Node
{
	public readonly float[] NikoScales = [0.5f, 1f, 2f, 3f, 4f];
	const double MEOW_TIME = 0.2;
	const double TIME_BEFORE_SLEEP = 0.5;

	private ValuesContainer _valuesContainer;
	private Window _mainWindow;
	private NikoSkinManager _skinManager;

	private double _clickTimer = 0;
	private bool _pressed = false;
	private uint _idleTime = 0;
	private NikoState _currentState = NikoState.Idle;

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

	public override void _Ready()
	{
		_valuesContainer = GetNode<ValuesContainer>("/root/ValuesContainer");
		_skinManager = GetNode<NikoSkinManager>("/root/NikoSkinManager");
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

		_valuesContainer.FacepickForced += FacepicId => DoWhatNikoNeedToDo();
		_valuesContainer.FacepickUnforced += DoWhatNikoNeedToDo;
		_valuesContainer.GlobalTimerTicked += GlobalTimerTicked;

		NikoAnimationPlayer.AnimationFinished += animName => DoWhatNikoNeedToDo();
		NikoAnimationPlayer.AnimationStarted += animName => DoWhatNikoNeedToDo();

		NikoSpriteNode.GuiInput += Event =>
		{
			if (Event is InputEventMouse EventMouse)
				if (EventMouse.IsPressed() && !_valuesContainer.IsFacepicForced)
				{
					Click();
				}
		};
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
				break;

			case NikoState.Idle:
				if (_valuesContainer.NikoScared)
					SetSprite(_valuesContainer.ScareFacepic);
				else
					SetSprite(_valuesContainer.IdleFacepic);
				break;
		}
	}


	private void DoWhatNikoNeedToDo()
	{
		if (NikoAnimationPlayer.IsPlaying())
		{
			_currentState = NikoState.InAnimation;
			return;
		}
		if (_valuesContainer.IsFacepicForced)
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
			NikoAnimationPlayer.Play("click");
			if (MeowSoundPlayer.Stream != Meows[_valuesContainer.CurrentMeowSoundId])
				MeowSoundPlayer.Stream = Meows[_valuesContainer.CurrentMeowSoundId];
			MeowSoundPlayer.Play();
		}
	}
	
	public void SetSprite(string spriteId)
	{
		float scale = NikoScales[_valuesContainer.NikoScale];
		Texture2D sprite = _skinManager.GetCurrentSkinSprite(spriteId);
		NikoSpriteNode.Texture = sprite;
		NikoSpriteNode.CustomMinimumSize = sprite.GetSize() * scale * _skinManager.GetCurrentSkinBaseScale();
	}

	// for animations
	public void SetScaredSpeakFacepic()
	{
		if (_valuesContainer.NikoScared)
			SetSprite(_valuesContainer.ScareSpeakFacepic);
		else
			SetSprite(_valuesContainer.SpeakFacepic);
	}
	public void SetIdleFacepic()
	{
		if (_valuesContainer.NikoScared)
			SetSprite(_valuesContainer.ScareFacepic);
		else
			SetSprite(_valuesContainer.IdleFacepic);
	}
}
