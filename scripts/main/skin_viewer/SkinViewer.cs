using Godot;

public partial class SkinViewer : Window
{
	[Export] public Button ExitButton;
	[Export] public Node2D TopBorder;
	[Export] public Node2D RightBorder;
	[Export] public Node2D BottomBorder;
	[Export] public Node2D LeftBorder;
	public override void _Ready()
	{
		ExitButton.Pressed += Hide;
		CloseRequested += Hide;
		VisibilityChanged += Update;
	}

	private void Update()
	{
		if (!Visible)
			return;

		int Screen = DisplayServer.WindowGetCurrentScreen(GetWindowId());
		Vector2I ScreenPos = DisplayServer.ScreenGetPosition(Screen);
		Vector2I ScreenSize = DisplayServer.ScreenGetSize(Screen);
		Rect2I ScreenFreeArea = DisplayServer.ScreenGetUsableRect(Screen);
		Position = ScreenPos;
		Size = ScreenSize - new Vector2I(0, 2);

		TopBorder.Position = new (TopBorder.Position.X, ScreenFreeArea.Position.Y - Size.Y / 2);
		BottomBorder.Position = new (BottomBorder.Position.X, ScreenFreeArea.Position.Y + ScreenFreeArea.Size.Y - Size.Y / 2);
		LeftBorder.Position = new (ScreenFreeArea.Position.X - Size.X / 2, LeftBorder.Position.Y);
		RightBorder.Position = new (ScreenFreeArea.Position.X + ScreenFreeArea.Size.X - Size.X / 2, RightBorder.Position.Y);
	}
}
