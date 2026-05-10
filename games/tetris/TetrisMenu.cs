using Godot;

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
	public Timer MusicTimer;

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
		ExitButton.Pressed += GameController.QueueFree;

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
		};

		MusicTimer.Timeout += () =>
		{
			//ProgressionPlayer.PlaySection("Progression", MusicPlayer.GetPlaybackPosition());
			StatisticBumpAnimator.Play("Bump2");
		};
	}
}
