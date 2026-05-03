using Godot;

public partial class NikoClickablePolygon : Node2D
{
	[Export]
	public Area2D ClickablePolygon;
	[Export]
	public Area2D MousePolygon;
	[Export]
	public TextureRect NikoSprite;
	public WindowExstantions _windowExstantions;
	public ValuesContainer _valuesContainer;
	
	private Vector2I MousePositionRelative;
	private bool m_old = false;

	public override void _Ready()
	{
		_valuesContainer = GetNode<ValuesContainer>("/root/ValuesContainer");
		_windowExstantions = GetNode<WindowExstantions>("/root/WindowExstantions");
	}


	public override void _Process(double delta)
	{
		MousePositionRelative = DisplayServer.MouseGetPosition() - DisplayServer.WindowGetPosition((int)DisplayServer.MainWindowId);
		MousePolygon.Position = MousePositionRelative;


		bool m_new = !MouseInClickableArea();
		if (m_new != m_old)
		{
			GetWindow().MousePassthrough = m_new;
			_windowExstantions.UpdateWindowsExStyles(GetWindow(), !_valuesContainer.ShowTaskbarIcon);
			m_old = m_new;
		}
	}

	public bool MouseInClickableArea()
	{
		var Polygons = ClickablePolygon.GetOverlappingAreas();
		if (Polygons.Contains(MousePolygon))
			return true && !_valuesContainer.GamingModeEnabled;

		Vector2 MousePositionOnRect = MousePositionRelative - (Vector2I)NikoSprite.Position;
		Image NikoImage = NikoSprite.Texture.GetImage();
		Vector2I ImageSize = NikoImage.GetSize();
		Vector2 MousePositionOnImage = MousePositionOnRect * ImageSize / new Vector2(NikoSprite.Size.Y, NikoSprite.Size.Y);
		if (MousePositionOnImage.X >= 0 
		 && MousePositionOnImage.Y >= 0
		 && MousePositionOnImage.X < ImageSize.X
		 && MousePositionOnImage.Y < ImageSize.Y)
			if (NikoImage.GetPixelv((Vector2I)MousePositionOnImage).A > 0.5f)
				return true;
		
		return false;
	}
}
