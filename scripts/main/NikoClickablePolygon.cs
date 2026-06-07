using Godot;

public partial class NikoClickablePolygon : Node
{
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
		if (_valuesContainer.GamingModeEnabled || !_valuesContainer.NikoVisible)
			return false;

		if (MousePositionRelative.Y >= NikoSprite.Size.Y && MousePositionRelative.Y < NikoSprite.Size.Y + 30)
			return true;

		Vector2 MousePositionOnRect = MousePositionRelative - (Vector2I)NikoSprite.Position;
		Image NikoImage = NikoSprite.Texture.GetImage();
		Vector2I ImageSize = NikoImage.GetSize();
		Vector2 MousePositionOnImage = MousePositionOnRect * ImageSize / NikoSprite.Size;
		if (MousePositionOnImage.X >= 0 
		 && MousePositionOnImage.Y >= 0
		 && MousePositionOnImage.X < ImageSize.X
		 && MousePositionOnImage.Y < ImageSize.Y)
		{
		 	if (NikoSprite.FlipH)
				MousePositionOnImage = new (ImageSize.X - MousePositionOnImage.X - 1, MousePositionOnImage.Y);
			if (NikoImage.GetPixelv((Vector2I)MousePositionOnImage).A > 0.5f)
				return true;
		}
		
		return false;
	}
}
