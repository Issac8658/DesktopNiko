using Godot;
using System;

public partial class CameraMove : Camera2D
{
	[Export]
	public float CameraMoveScale = 1;
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotion)
		{
			Position = (mouseMotion.Position - GetWindow().Size / 2) * CameraMoveScale + GetWindow().Size / 2;
		}
	}

}
