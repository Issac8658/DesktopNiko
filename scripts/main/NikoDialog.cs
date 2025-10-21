using System;
using System.Reflection.Metadata.Ecma335;
using Godot;

public partial class NikoDialog : Window
{
	[Signal]
	delegate void DialogSkippedEventHandler();
	[Signal]
	delegate void ShowNextRequestEventHandler();
	[Signal]
	delegate void ButtonPressedEventHandler(short ButtonId);

	const int MainWindowId = (int)DisplayServer.MainWindowId;
	[Export]
	public RichTextLabel DialogLabel;
	[Export]
	public Timer TextShowTimer;
	[Export]
	public AudioStreamPlayer TextSound;
	[Export]
	public Control ClickControl;
	[Export]
	public Control ButtonsContainer;
	[Export]
	public Control Hint;
	private Callable ShowSymbolCallable;
	private Vector2I DialogSize = new(300, 140);
	private bool CanSkip = true;

	public override void _Ready()
	{
		ShowSymbolCallable = Callable.From(ShowNextSymbol);
		Visible = false;
		Hint.Visible = false;
		DialogLabel.VisibleCharacters = 0;
		Size = new(300, 140);
		TextShowTimer.Connect("timeout", ShowSymbolCallable);
		
		ClickControl.GuiInput += Event =>
		{
			if (Event.IsPressed())
			{
				TrySkipDialog();

				GD.Print("Dialog Pressed");
			}
		};
	}

	public void ShowDialog(Vector2I TargetDialogSize, string Text = "Nothing beats a jet2holidays and right now you can save fifty pounds per person", string[] Buttons = null, float SymbolShowTime = 0.05f)
	{
		RemoveButtons();
		Hint.Visible = false;
		if (Buttons == null) {
			ButtonsContainer.Visible = false;
			CanSkip = true;
		}
		else {
			ButtonsContainer.Visible = true;
			CanSkip = false;
			CreateButtons(Buttons);
		}

		DialogLabel.VisibleCharacters = 0;
		DialogLabel.Text = "";
		Size = new(114, 114);
		Position = GetNikoPosition();
		DialogSize = TargetDialogSize;
		DialogLabel.Text = Text;
		Visible = true;

		TextShowTimer.WaitTime = SymbolShowTime;
		TextShowTimer.Start();
	}

	public void EndDialog()
	{
		TextShowTimer.Stop();
		Visible = false;
		DialogLabel.VisibleCharacters = 0;
		EmitSignal("DialogSkipped");
	}

	public void ShowNewText(Vector2I TargetDialogSize, string Text, string[] Buttons = null, float SymbolShowTime = 0.05f)
	{
		Hint.Visible = false;
		DialogLabel.VisibleCharacters = 0;
		DialogLabel.Text = "";
		ShowMoreText(TargetDialogSize, Text, Buttons, SymbolShowTime);
	}
	
	public void ShowMoreText(Vector2I TargetDialogSize, string Text, string[] Buttons = null, float SymbolShowTime = 0.05f)
	{
		Hint.Visible = false;
		RemoveButtons();
		if (Buttons == null) {
			ButtonsContainer.Visible = false;
			CanSkip = true;
		}
		else {
			ButtonsContainer.Visible = true;
			CanSkip = false;
			CreateButtons(Buttons);
		}
		Visible = true;

		DialogSize = TargetDialogSize;
		DialogLabel.Text += Text;

		TextShowTimer.WaitTime = SymbolShowTime;
		TextShowTimer.Start();
	}

	private void ShowNextSymbol()
	{
		DialogLabel.VisibleCharacters += 1;
		TextSound.Play();
		if (DialogLabel.VisibleCharacters >= DialogLabel.GetParsedText().Length)
		{
			TextShowTimer.Stop();
			ShowHint();
		}
	}
	private void TrySkipDialog()
	{
		if (DialogLabel.VisibleCharacters < DialogLabel.GetParsedText().Length)
		{
			TextShowTimer.Stop();
			ShowHint();
			DialogLabel.VisibleCharacters = DialogLabel.GetParsedText().Length;
		}
		else if (CanSkip)
		{
			TextShowTimer.Stop();
			EmitSignal("ShowNextRequest");
		}
	}

	private void CreateButtons(string[] Buttons)
	{
		for (int i = 0; i < Buttons.Length; i++)
		{
			CreateButton(i, Buttons[i]);
		}
	}
	private void CreateButton(int ButtonId, string Text = "")
	{
		Button DialogButton = new() { Text = Text, SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
		ButtonsContainer.AddChild(DialogButton);
		DialogButton.Pressed += () =>
		{
			EmitSignal("ButtonPressed", (short)ButtonId);
		};
	}
	private void RemoveButtons()
	{
		foreach (Node Child in ButtonsContainer.GetChildren())
			Child.QueueFree();
	}

	private void ShowHint()
	{
		if (CanSkip) 
			Hint.Visible = true;
	}

	//------------------------------------------------------------------------------------------------------

	// Window Move Animation Parameters
	[Export]
	public Texture2D[] ArrowsVariants;
	[ExportGroup("Upper Panel", "Upper")]
	[Export]
	public TextureRect UpperArrow;
	[Export]
	public Control UpperArrowControl;
	[Export]
	public Control UpperFramePart1;
	[Export]
	public Control UpperFramePart2;
	[ExportGroup("Lower Panel", "Lower")]
	[Export]
	public TextureRect LowerArrow;
	[Export]
	public Control LowerArrowControl;
	[Export]
	public Control LowerFramePart1;
	[Export]
	public Control LowerFramePart2;
	[ExportGroup("Positons", "Position")]
	[Export]
	public Vector2I PositionTopLeft = new();
	[Export]
	public Vector2I PositionTopRight = new();
	[Export]
	public Vector2I PositionBottomLeft = new();
	[Export]
	public Vector2I PositionBottomRight = new();
	public override void _Process(double delta)
	{
		if (Visible)
		{
			Size = Lerp(Size, DialogSize, 0.3f);

			Vector2I NikoPos = GetNikoPosition();
			Position = Lerp(Position, GetFreePos(NikoPos) + NikoPos, 0.1f);

			NikoPos += DisplayServer.WindowGetSize(MainWindowId) / 2;
			Vector2I SizeDiv2 = Size / 2;
			if (NikoPos.X < Position.X + SizeDiv2.X)
			{
				UpperArrow.FlipH = false;
				LowerArrow.FlipH = false;
			}
			else
			{
				UpperArrow.FlipH = true;
				LowerArrow.FlipH = true;
			}
			if (NikoPos.Y < Position.Y + SizeDiv2.Y)
			{
				LowerArrowControl.Visible = false;
				UpperArrowControl.Visible = true;
			}
			else
			{
				LowerArrowControl.Visible = true;
				UpperArrowControl.Visible = false;
			}
			int Distance = Position.X + SizeDiv2.X - NikoPos.X;
			int ArrowVariant = (int)Mathf.Clamp(5f - Mathf.Round(Mathf.Abs(Distance) / (SizeDiv2.X / 4.0)), 0f, 5f);
			UpperArrow.Texture = ArrowsVariants[ArrowVariant];
			LowerArrow.Texture = ArrowsVariants[ArrowVariant];
			float Stretch = Mathf.Clamp(Distance + SizeDiv2.X, 0f, Size.X) / Size.X * 100f;
			UpperFramePart1.SizeFlagsStretchRatio = 100f - Stretch;
			UpperFramePart2.SizeFlagsStretchRatio = Stretch;
			LowerFramePart1.SizeFlagsStretchRatio = 100f - Stretch;
			LowerFramePart2.SizeFlagsStretchRatio = Stretch;
		}
	}

	private Vector2I GetFreePos(Vector2I NikoPos)
	{
		Vector2I ScreenPosition = DisplayServer.ScreenGetPosition(DisplayServer.WindowGetCurrentScreen(MainWindowId));
		bool ToBottom = false;
		bool ToRight = false;
		if (NikoPos.Y - ScreenPosition.Y < Size.Y)
		{
			ToBottom = true;
		}
		if (NikoPos.X - ScreenPosition.X < Size.X)
		{
			ToRight = true;
		}
		if (ToBottom && ToRight)
		{
			return PositionBottomRight;
		}
		if (ToBottom)
		{
			return PositionBottomLeft - new Vector2I(Size.X, 0);
		}
		if (ToRight)
		{
			return PositionTopRight - new Vector2I(0, Size.Y);
		}
		return PositionTopLeft - Size;
	}

	private static Vector2I Lerp(Vector2I a, Vector2I b, float t)
	{
		return (Vector2I)(a * new Vector2(1 - t, 1 - t) + new Vector2(t, t) * b);
	}
	
	private static Vector2I GetNikoPosition()
	{
		return DisplayServer.WindowGetPosition(MainWindowId);
	}
}
