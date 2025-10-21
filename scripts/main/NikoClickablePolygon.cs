using System.Linq;
using Godot;

public partial class NikoClickablePolygon : Node2D
{
	[Export]
	public Area2D ClickablePolygon;
	[Export]
	public Area2D MousePolygon;
	public WindowMousePassthroughModule PassthroughModule;
	public ValuesContainer ValuesContainer;

	private bool m_old = false;

	public override void _Ready()
	{
		ValuesContainer = GetNode("/root/ValuesContainer") as ValuesContainer;
		PassthroughModule = GetNode("/root/PassthroughModule") as WindowMousePassthroughModule;
	}


	public override void _Process(double delta)
	{
		MousePolygon.Position = DisplayServer.MouseGetPosition() - DisplayServer.WindowGetPosition((int)DisplayServer.MainWindowId);


		bool m_new = !MouseInClickableArea();
		if (m_new != m_old)
		{
			GetWindow().MousePassthrough = m_new;
			PassthroughModule.UpdateWindowsExStyles(GetWindow(), !ValuesContainer.ShowTaskbarIcon, true);
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
