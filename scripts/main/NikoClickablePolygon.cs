using System.Linq;
using Godot;
using Godot.NativeInterop;

public partial class NikoClickablePolygon : Node2D
{
	[Export]
	public Area2D ClickablePolygon;
	[Export]
	public Area2D MousePolygon;
	[Export]
	public CollisionPolygon2D Polygon;
	public WindowExstantions _windowExstantions;
	public ValuesContainer _valuesContainer;

	private bool m_old = false;

	public override void _Ready()
	{
		_valuesContainer = GetNode<ValuesContainer>("/root/ValuesContainer");
		_windowExstantions = GetNode<WindowExstantions>("/root/WindowExstantions");
		GD.Print(new Godot.Collections.Array<Vector2>(Polygon.Polygon));
	}


	public override void _Process(double delta)
	{
		MousePolygon.Position = DisplayServer.MouseGetPosition() - DisplayServer.WindowGetPosition((int)DisplayServer.MainWindowId);


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
		if (Polygons.ToArray().Contains(MousePolygon))
		{
			//GD.Print("Yay, Mouse can pat Niko!");
			return true && !_valuesContainer.GamingModeEnabled;
		}
		return false;
	}
}
