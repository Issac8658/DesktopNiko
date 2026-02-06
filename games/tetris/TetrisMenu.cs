using Godot;
using System;

public partial class TetrisMenu : Node
{
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
	public Timer GameTimer;

	public override void _Ready()
	{
		PlayButton.Pressed += () =>
		{
			MenuWindow.Visible = false;
			if (GameController.CurrentState == TetrisGameController.GameStates.Menu)
				GameController.CurrentState = TetrisGameController.GameStates.Playing;
			else
				GameController.CurrentState = TetrisGameController.GameStates.Restart;
			if (MusicCheckbox.ButtonPressed)
			{
				MusicPlayer.Play();
			}
		};
		ExitButton.Pressed += GameController.QueueFree;

		MenuWindow.Visible = true;

		GameController.StateChanged += () =>
		{
			if (GameController.CurrentState == TetrisGameController.GameStates.Lose)
			{
				MenuWindow.Visible = true;
				PlayButton.Text = "RESTART";
				MusicPlayer.Stop();
			}
			if (GameController.CurrentState == TetrisGameController.GameStates.Menu)
			{
				MenuWindow.Visible = true;
				PlayButton.Text = "PLAY";
				MusicPlayer.StreamPaused = true;
			}
		};

		GameTimer.Timeout += () =>
		{
			//ProgressionPlayer.PlaySection("Progression", MusicPlayer.GetPlaybackPosition());
			StatisticBumpAnimator.Play("Bump2");
		};
	}
}
