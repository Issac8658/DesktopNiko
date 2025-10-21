using Godot;
using System;

public partial class NikoController : Node
{
	const double MEOW_TIME = 0.2;

	[Export]
	public AudioStream[] Meows = [];
	[Export]
	public Control MenuPanel;
	[Export]
	public Button MenuButton;
	[Export]
	public Window WindowToOpen;
	[Export]
	public TextureRect NikoPanel;
	[Export]
	public AudioStreamPlayer MeowSoundPlayer;

	private ValuesContainer ValuesContainer;
	private Window MainWindow;

	private double ClickTimer = 0;
	private bool pressed = false;

	public override void _Ready()
	{
		ValuesContainer = GetNode("/root/ValuesContainer") as ValuesContainer;
		MainWindow = GetWindow();

		MainWindow.MouseEntered += () =>
		{
			MenuPanel.Modulate = Color.Color8(255, 255, 255, 255);
			ValuesContainer.NikoHovered = true;
		};
		MainWindow.MouseExited += () =>
		{
			MenuPanel.Modulate = Color.Color8(255, 255, 255, 0);
			ValuesContainer.NikoHovered = false;
		};

		MenuButton.Pressed += () =>
		{
			WindowToOpen.Visible = !WindowToOpen.Visible;
		};

		NikoPanel.GuiInput += Event =>
		{
			if (Event is InputEventMouse)
			{
				if (Event.IsPressed() && !ValuesContainer.IsFacepickForced)
				{
					Click();
					if (MeowSoundPlayer.Stream != Meows[ValuesContainer.CurrentMeowSoundId])
						MeowSoundPlayer.Stream = Meows[ValuesContainer.CurrentMeowSoundId];
					MeowSoundPlayer.Play();
				}
			}
		};
	}

	public override void _Process(double delta)
	{
		if (pressed)
		{
			if (ClickTimer > 0)
			{
				pressed = true;
				ClickTimer -= delta;
			}
			else
			{
				pressed = false;
				UpdateFacepick();
			}
		}
	}


	private void UpdateFacepick()
	{
		if (ValuesContainer.IsFacepickForced)
		{
			
		}
	}
	
	private void Click()
	{
		if (!ValuesContainer.IsFacepickForced)
		{
			ValuesContainer.Clicks += 1;
			ClickTimer = MEOW_TIME;
			pressed = true;
			UpdateFacepick();
		}
	}
}
