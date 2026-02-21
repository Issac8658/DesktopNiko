using Godot;

public partial class NikoWindowControl : Control
{
	public readonly float[] NikoScales = [0.5f, 1f, 2f, 3f, 4f];
	
	private Window _mainWindow;
	private GlobalController _globalController;
	private ValuesContainer _valuesContainer;
	private NikoSkinManager _skinManager;

	[Export]
	public Control NikoClickControl;
	[Export]
	public TextureRect NikoSpriteNode;
	[Export]
	public Node2D AreasNode;
	[Export]
	public Node2D SleepParticles;

	//Window drag vars
	private bool IsDragging = false;
	private Vector2I MouseOffset = new();

	public override void _Ready()
	{
		_mainWindow = GetWindow();
		_valuesContainer = GetNode("/root/ValuesContainer") as ValuesContainer;
		_skinManager = GetNode<NikoSkinManager>("/root/NikoSkinManager");
		//MainWindow.WrapControls = true;

		_valuesContainer.NikoScaleChanged += (NikoScale) => UpdateScale();

		ItemRectChanged += () => _mainWindow.Size = (Vector2I)Size;

		NikoClickControl.GuiInput += Event =>
		{
			if (Event is InputEventMouseButton)
				if ((Event as InputEventMouseButton).ButtonIndex == MouseButton.Left)
					if (Event.IsPressed())
					{
						MouseOffset = DisplayServer.MouseGetPosition() - _mainWindow.Position;
						IsDragging = true;
					}
					else
						IsDragging = false;
		};
	}

	public override void _Process(double delta)
	{
		if (IsDragging)
		{
			_mainWindow.Position = DisplayServer.MouseGetPosition() - MouseOffset;
		}
	}


	public void UpdateScale()
	{
		float scale = NikoScales[_valuesContainer.NikoScale];
		SleepParticles.Scale = AreasNode.Scale = new(scale, scale);
		_mainWindow.Size = new((int)Size.X, (int)Size.Y);
		NikoSpriteNode.CustomMinimumSize = NikoSpriteNode.Texture.GetSize() * scale * _skinManager.GetCurrentSkinBaseScale();
	}
}
