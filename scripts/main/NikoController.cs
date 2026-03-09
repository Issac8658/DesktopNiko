using Godot;

public enum NikoState {Idle, Clicking, Sleepy, Sleeping, ForcedFacepic, InAnimation}

public partial class NikoController : Node
{
	public readonly float[] NikoScales = [0.5f, 1f, 2f, 3f, 4f];
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

		_valuesContainer.FacepickForced += FacepicId => DoWhatNikoNeedToDo();
		_valuesContainer.FacepickUnforced += DoWhatNikoNeedToDo;
		_valuesContainer.GlobalTimerTicked += GlobalTimerTicked;

		NikoAnimationPlayer.AnimationFinished += animName => DoWhatNikoNeedToDo();
		NikoAnimationPlayer.AnimationStarted += animName => DoWhatNikoNeedToDo();

		NikoSpriteNode.GuiInput += Event =>
		{
			if (Event is InputEventMouseButton EventMouse)
				if (EventMouse.IsPressed() && EventMouse.ButtonIndex == MouseButton.Left && !_valuesContainer.IsFacepicForced)
				{
					Click();
					// GetNode<NikoDialog>("/root/NikoDialog").ShowDialog(new(200, 60), "meow");
				}
		};
		GetNode<NikoDialog>("/root/NikoDialog").ShowNextRequest += () =>
		{
			GetNode<NikoDialog>("/root/NikoDialog").EndDialog();
		};
		DoWhatNikoNeedToDo();
		
		_skinManager.SkinChanged += SkinId => UpdateFacepic();
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
				if (!_achievementsController.IsAchievementTaked("sweet_dreams"))
				{
					_achievementsController.TakeAchievement("sweet_dreams");
				}
				break;

			case NikoState.Sleepy:
				SetSprite("sleepy");
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
		_valuesContainer.NikoScared = _valuesContainer.CPS > 8;

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
			NikoAnimationPlayer.Stop();
			NikoAnimationPlayer.Play("click");
			if (MeowSoundPlayer.Stream != Meows[_valuesContainer.CurrentMeowSoundId])
				MeowSoundPlayer.Stream = Meows[_valuesContainer.CurrentMeowSoundId];
			MeowSoundPlayer.Play();
		
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
