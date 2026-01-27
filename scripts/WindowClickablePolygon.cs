using System.Linq;
using Godot;

public partial class WindowClickablePolygon : Node2D
{
	[Export]
	public Area2D ClickablePolygon;
	[Export]
	public Area2D MousePolygon;
	[Export]
	public bool ShowTaskbarIcon;

	private WindowMousePassthroughModule _passthroughModule;
	private Window _window;

	private bool m_old = false;

	public override void _Ready()
	{
		_passthroughModule = GetNode("/root/PassthroughModule") as WindowMousePassthroughModule;
		_window = GetWindow();
	}


	public override void _Process(double delta)
	{
		MousePolygon.Position = DisplayServer.MouseGetPosition() - _window.Position;


		bool m_new = !MouseInClickableArea();
		if (m_new != m_old)
		{
			_window.MousePassthrough = m_new;
			_passthroughModule.UpdateWindowsExStyles(_window, !ShowTaskbarIcon, true);
			m_old = m_new;
		}
	}

	public bool MouseInClickableArea()
	{
		var Polygons = ClickablePolygon.GetOverlappingAreas();
		if (Polygons.ToArray().Contains(MousePolygon))
		{
			//GD.Print("Yay, Mouse can pat Niko!");
			return true;
		}
		return false;
	}
}
