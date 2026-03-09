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
		Vector2I SPos = DisplayServer.ScreenGetPosition(DisplayServer.GetPrimaryScreen());
		Vector2I SSize = DisplayServer.ScreenGetSize(DisplayServer.GetPrimaryScreen());
		Rect2I SFree = DisplayServer.ScreenGetUsableRect(DisplayServer.GetPrimaryScreen());
		Position = SPos;
		Size = SSize - new Vector2I(0, 2);

		TopBorder.Position = new (TopBorder.Position.X, SFree.Position.Y - Size.Y / 2);
		BottomBorder.Position = new (BottomBorder.Position.X, SFree.Position.Y + SFree.Size.Y - Size.Y / 2);
		LeftBorder.Position = new (SFree.Position.X - Size.X / 2, LeftBorder.Position.Y);
		RightBorder.Position = new (SFree.Position.X + SFree.Size.X - Size.X / 2, RightBorder.Position.Y);

		ExitButton.Pressed += Hide;
	}
}
