using Godot;
using System;

public partial class SkinPreview : RigidBody2D
{
	private Texture2D _skinImage;
	private string _displayText = "";
	private float _skinScale;
	private Vector2 _oldCameraPos;
	private Camera2D _camera;

	private bool Dragging = false;
	private Vector2 DragPosition = new();
	private Vector2 MouseOffset = new();

	[Export]
	public CollisionShape2D CollisionShape;
	[Export]
	public Control MonitorControl;
	[Export]
	public TextureRect PreviewImageControl;
	[Export]
	public Label NameLabel;
	[Export]
	public Texture2D SkinImage
	{
		set
		{
			_skinImage = value;
			if (PreviewImageControl != null)
			{
				PreviewImageControl.Texture = value;
				PreviewImageControl.CustomMinimumSize = value.GetSize() * _skinScale;
			}
		}
		get => _skinImage;
	}
	[Export]
	public string DisplayText
	{
		set
		{
			_displayText = value;
			NameLabel.Text = value;
		}
		get => _displayText;
	}
	[Export]
	public float SkinScale
	{
		set
		{
			_skinScale = value;
			if (PreviewImageControl != null)
				PreviewImageControl.CustomMinimumSize = PreviewImageControl.Texture.GetSize() * value; // replace with skin scale
		}
		get => _skinScale;
	}
	public string OriginalSkinId;

	public override void _Ready()
	{
		_camera = GetViewport().GetCamera2D();
		MonitorControl.GuiInput += Event =>
		{
			if (Event is InputEventMouseButton MouseButton)
			{
				_oldCameraPos = _camera.GlobalPosition;
				MouseOffset = MouseButton.GlobalPosition - Position;
				Dragging = MouseButton.IsPressed();
			}
		};
	}
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			DragPosition = eventMouseMotion.GlobalPosition;
		}
	}

	public override void _Process(double delta)
	{
		(CollisionShape.Shape as RectangleShape2D).Size = MonitorControl.Size;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Dragging)
			ApplyImpulse(DragPosition - Position - MouseOffset + (_camera.Position - _oldCameraPos));
	}
}
