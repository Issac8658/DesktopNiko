using System.Linq;
using Godot;

public partial class NikoClickablePolygon : Node2D
{
	[Export]
	public Area2D ClickablePolygon;
	[Export]
	public Area2D MousePolygon;
	public WindowExstantions WindowExstantions;
	public ValuesContainer ValuesContainer;

	private bool m_old = false;

	public override void _Ready()
	{
		ValuesContainer = GetNode<ValuesContainer>("/root/ValuesContainer");
		WindowExstantions = GetNode<WindowExstantions>("/root/WindowExstantions");
	}


	public override void _Process(double delta)
	{
		MousePolygon.Position = DisplayServer.MouseGetPosition() - DisplayServer.WindowGetPosition((int)DisplayServer.MainWindowId);


		bool m_new = !MouseInClickableArea();
		if (m_new != m_old)
		{
			GetWindow().MousePassthrough = m_new;
			WindowExstantions.UpdateWindowsExStyles(GetWindow(), !ValuesContainer.ShowTaskbarIcon);
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
