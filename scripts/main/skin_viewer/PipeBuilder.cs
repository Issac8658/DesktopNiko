using Godot;
using System;
using System.Collections.Generic;

public partial class PipeBuilder : Node
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

	public override void _Ready()
	{
		MonitorsContainer.ChildEnteredTree += Child =>
		{
			if (Child is Node2D Monitor)
				CreatePipe(Monitor);
		};
		foreach (Node node in MonitorsContainer.GetChildren())
		{
			if (node is Node2D Monitor)
				CreatePipe(Monitor);
		}
	}

	public override void _Process(double delta)
	{
		foreach (Line2D Pipe in Pipes.Keys)
		{
			(Node2D, Vector2, bool) Linked = Pipes[Pipe];

			if (Linked.Item3)
				Pipe.Points = [
					Linked.Item1.GlobalPosition,
					new(From.GlobalPosition.X + Linked.Item2.X, Linked.Item1.GlobalPosition.Y),
					From.GlobalPosition + Linked.Item2
				];
			else
				Pipe.Points = [
					Linked.Item1.GlobalPosition,
					new(Linked.Item1.GlobalPosition.X, From.GlobalPosition.Y + Linked.Item2.Y),
					From.GlobalPosition + Linked.Item2
				];
		}
	}

	
	private void CreatePipe(Node2D LinkedNode)
	{
		Line2D Pipe = PipeTemplate.Instantiate() as Line2D;
		Pipe.Position = new();
		AddChild(Pipe);
		Pipes.Add(Pipe, (LinkedNode, new Vector2((GD.Randf() - 0.5f) * 2f, (GD.Randf() - 0.5f) * 2f) * MaxRandomOffset, GD.Randi() % 2 == 1));

		LinkedNode.TreeExiting += () =>
		{
			Pipes.Remove(Pipe);
		};
	}
}
