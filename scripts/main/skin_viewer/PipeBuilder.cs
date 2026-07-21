using Godot;
using System;
using System.Collections.Generic;

public partial class PipeBuilder : Node2D
{
	[Export]
	public PackedScene PipeTemplate;
	[Export]
	public Node2D From;
	[Export]
	public Node MonitorsContainer;
	[Export]
	public Vector2 MaxRandomOffset = new();

	private Dictionary<Line2D, (Node2D, Vector2, bool)> Pipes = [];

	public override void _Process(double delta)
	{
		foreach (Line2D Pipe in Pipes.Keys)
		{
			(Node2D, Vector2, bool) Linked = Pipes[Pipe];

			if (Linked.Item3)
				Pipe.Points = [
					Linked.Item1.GlobalPosition - From.GlobalPosition,
					new(Linked.Item2.X, Linked.Item1.GlobalPosition.Y - From.GlobalPosition.Y),
					Linked.Item2
				];
			else
				Pipe.Points = [
					Linked.Item1.GlobalPosition - From.GlobalPosition,
					new(Linked.Item1.GlobalPosition.X - From.GlobalPosition.X, Linked.Item2.Y),
					Linked.Item2
				];
		}
	}

	
	public void CreatePipe(Node2D LinkedNode, Node Parent)
	{
		Line2D Pipe = PipeTemplate.Instantiate() as Line2D;
		Pipe.Position = new();
		Parent.AddChild(Pipe);
		Pipes.Add(Pipe, (LinkedNode, new Vector2((GD.Randf() - 0.5f) * 2f, (GD.Randf() - 0.5f) * 2f) * MaxRandomOffset, GD.Randi() % 2 == 1));

		LinkedNode.TreeExiting += () =>
		{
			Pipes.Remove(Pipe);
		};
	}
}
// sfsf
