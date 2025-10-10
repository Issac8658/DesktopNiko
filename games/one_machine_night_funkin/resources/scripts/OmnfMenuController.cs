using Godot;
using System;
using System.Collections.Generic;

public partial class OmnfMenuController : Node
{
	public enum MenuState { IntroText, TitleScreen, Menu, StoryMode, FreePlay, Settings, About }
	[Export]
	public AnimationPlayer TitleScreenAnimator;
	[Export]
	public AnimationPlayer UIAnimator;
	[Export]
	public AnimationPlayer IntroTextAnimator;
	[Export]
	public OmnfSongPlayer SongPlayer;
	[Export]
	public OmnfMenuButtonsController MenuButtonsController;
	[Export]
	public AudioStreamPlayer MenuDecision;
	[Export]
	public AudioStreamPlayer MenuCancel;
	[Export]
	public AudioStreamPlayer MenuCursor;
	[Export]
	public Node GameNode;
	[Export]
	public string PathToIntroText;
	[Export]
	public Label[] IntroTextLabels;
	private string[] IntroTexts;
	private int CurrentShowedIntroText = -1;
	private int MenuDanceTimer = 0;

	public MenuState CurrentMenuState = MenuState.IntroText;
	public override void _Ready()
	{
		using var file = FileAccess.Open(PathToIntroText, FileAccess.ModeFlags.Read);
		IntroTexts = file.GetAsText().Split("\n");

		GetNode("/root/GlobalControlls").Call("set_niko_visibility", false);
		SongPlayer.Beat += Beat;
		SongPlayer.PlaySong("FreakyMachine");
		TitleScreenAnimator.SpeedScale = SongPlayer.CurrentSong.Tempo / 60f / 2.0f;

		for (int i = 0; i < IntroTextLabels.Length; i++)
		{
			IntroTextLabels[i].Text = "";
		}
	}

	public override void _Input(InputEvent Event)
	{
		if (Event.IsActionPressed("dialog_skip"))
		{
			switch (CurrentMenuState)
			{
				case MenuState.TitleScreen:
					CurrentMenuState = MenuState.Menu;
					UIAnimator.Play("CameraToMenu");
					MenuDecision.Play();
					break;
				case MenuState.Menu:
					if (CurrentMenuState == MenuState.Menu)
					{
						switch (MenuButtonsController.CurrentButton)
						{
							case 0:
								CurrentMenuState = MenuState.StoryMode;
								UIAnimator.Play("MenuToStoryMode");
								break;
						}
					}
					MenuDecision.Play();
					break;
			}
		}
		else if (Event.IsActionPressed("back"))
		{
			switch (CurrentMenuState)
			{
				case MenuState.TitleScreen:
					GetNode("/root/GlobalControlls").Call("set_niko_visibility", true);
					GameNode.QueueFree();
					break;
				case MenuState.Menu:
					CurrentMenuState = MenuState.TitleScreen;
					UIAnimator.Play("ToTitleScreen");
					MenuCancel.Play();
					break;
				case MenuState.StoryMode:
					CurrentMenuState = MenuState.Menu;
					UIAnimator.Play("StoryModeToMenu");
					MenuCancel.Play();
					break;
			}
		}
	}

	private void Beat()
	{
		if (CurrentShowedIntroText < IntroTexts.Length && CurrentShowedIntroText >= 0)
		{
			ShowMoreText();
		}
		if (CurrentShowedIntroText >= IntroTexts.Length - 2)
		{
			if (MenuDanceTimer == 1)
			{
				MenuDanceTimer = 0;
				TitleScreenAnimator.Stop();
				TitleScreenAnimator.Play("MenuDance");
			}
			else
			{
				MenuDanceTimer = 1;
			}
		}
		if (CurrentShowedIntroText == IntroTexts.Length - 1)
		{
			CurrentMenuState = MenuState.TitleScreen;
			IntroTextAnimator.Play("TitleBoom");
			CurrentShowedIntroText += 1;
		}
		CurrentShowedIntroText += 1;
	}

	private void ShowMoreText()
	{
		int StartText = (int)Mathf.Floor(CurrentShowedIntroText / IntroTextLabels.Length) * IntroTextLabels.Length;
		for (int i = 0; i < IntroTextLabels.Length; i++)
		{
			if (StartText + i <= CurrentShowedIntroText)
			{
				IntroTextLabels[i].Text = IntroTexts[StartText + i];
			}
			else
			{
				IntroTextLabels[i].Text = "";
			}
		}
	}
}
