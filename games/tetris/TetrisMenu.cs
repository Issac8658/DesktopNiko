using Godot;
using Tetris;

public partial class TetrisMenu : Node
{
	private double _pauseColtdown;

	[Export]
	public TetrisGameController GameController;
	[Export]
	public Window MenuWindow;
	[Export]
	public AudioStreamPlayer MusicPlayer;

	[Export]
	public Button PlayButton;
	[Export]
	public CheckBox MusicCheckbox;
	[Export]
	public Button ExitButton;
	[Export]
	public AnimationPlayer StatisticBumpAnimator;
	[Export]
	public Timer MusicTimer;
	[Export]
	public Control PausePanel;

	public override void _Ready()
	{

		PlayButton.Pressed += () =>
		{
			MenuWindow.Visible = false;
			if (GameController.CurrentState == TetrisGameController.GameStates.Menu)
				GameController.CurrentState = TetrisGameController.GameStates.Playing;
			else
				GameController.CurrentState = TetrisGameController.GameStates.Restart;
			if (!MusicCheckbox.ButtonPressed)
			{
				MusicPlayer.Stop();
				MusicTimer.Stop();
			}
			else if (!MusicPlayer.Playing)
			{
				MusicPlayer.Play();
				MusicTimer.Start();
			}
		};
		ExitButton.Pressed += () => {
			(AudioServer.GetBusEffect(2, 0) as AudioEffectLowPassFilter).CutoffHz = 20500;
			GameController.QueueFree();
		};

		MenuWindow.Visible = true;

		GameController.StateChanged += () =>
		{
			if (GameController.CurrentState == TetrisGameController.GameStates.Lose)
			{
				MenuWindow.Visible = true;
				PlayButton.Text = "shared.restart";
				MusicPlayer.Stop();
				MusicTimer.Stop();
			}

			if (GameController.CurrentState == TetrisGameController.GameStates.Menu)
			{
				MenuWindow.Visible = true;
				PlayButton.Text = "shared.play";
			}

			if (GameController.CurrentState != TetrisGameController.GameStates.Playing)
				TweenPause(Colors.White);
			else
				TweenPause(Colors.Transparent);
		};

		MusicTimer.Timeout += () =>
		{
			//ProgressionPlayer.PlaySection("Progression", MusicPlayer.GetPlaybackPosition());
			StatisticBumpAnimator.Play("Bump2");
		};
	}

	
	private void TweenPause(Color result)
	{
		Tween tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Quint);
		tween.TweenProperty(PausePanel, "modulate", result, 0.25);
	}

	private void ResetPosition()
	{
		int Screen = DisplayServer.WindowGetCurrentScreen(GameController.GetWindowId());
		MenuWindow.Position = DisplayServer.ScreenGetPosition(Screen) + DisplayServer.ScreenGetSize(Screen) / 2 - MenuWindow.Size/2;
	}
}
