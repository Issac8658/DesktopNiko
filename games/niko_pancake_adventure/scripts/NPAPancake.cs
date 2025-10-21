using Godot;
using System;

public partial class NPAPancake : Area2D
{
	[Export]
	public Node2D VisualNode;

	private double time = 0;
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		time += delta;
		VisualNode.Position = new(0, (float)Mathf.Sin(time) * 10f);
	}
}
