using Godot;

public partial class NikoWindowControl : Control
{
	public readonly float[] NikoScales = [0.5f, 1f, 2f, 3f, 4f];
	public readonly Vector2 DefaultNikoSize = new(120, 120); // Will Replaced with skin system

	[Export]
	public Control NikoClickControl;
	[Export]
	public Node2D AreasNode;
	[Export]
	public Node2D SleepParticles;

	private Window MainWindow;
	private GlobalController GlobalController;
	private ValuesContainer ValuesContainer;

	//Window drag vars
	private bool IsDragging = false;
	private Vector2I MouseOffset = new();

	public override void _Ready()
	{
		MainWindow = GetWindow();
		ValuesContainer = GetNode("/root/ValuesContainer") as ValuesContainer;
		//MainWindow.WrapControls = true;

		ValuesContainer.NikoScaleChanged += (NikoScale) => UpdateScale(NikoScales[NikoScale]);

		ItemRectChanged += () => MainWindow.Size = (Vector2I)Size;

		NikoClickControl.GuiInput += Event =>
		{
			if (Event is InputEventMouseButton)
				if ((Event as InputEventMouseButton).ButtonIndex == MouseButton.Left)
					if (Event.IsPressed())
					{
						MouseOffset = DisplayServer.MouseGetPosition() - MainWindow.Position;
						IsDragging = true;
						UpdateScale(NikoScales[ValuesContainer.NikoScale]);
					}
					else
						IsDragging = false;
		};

		UpdateScale(NikoScales[ValuesContainer.NikoScale]);
	}

	public override void _Process(double delta)
	{
		if (IsDragging)
		{
			MainWindow.Position = DisplayServer.MouseGetPosition() - MouseOffset;
		}
	}


	public void UpdateScale(float Scale)
	{
		SetDeferred("size", Vector2I.Zero);
		SetDeferred("position", Vector2I.Zero);
		NikoClickControl.CustomMinimumSize = (Vector2I)(DefaultNikoSize * Scale);
		SleepParticles.Scale = AreasNode.Scale = new(Scale, Scale);
	}
}
