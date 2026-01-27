using Godot;
using System;

public partial class TetrisParticles : Node2D
{
	[Export]
	public TetrisGameController GameController;

	[Export]
	public PackedScene DestructionParticlesPrefab;

	public override void _Ready()
	{
		Position = GetWindow().Size / 2; // Move Particles Container to center
		GameController.LineDestroyed += Line =>
		{
			GpuParticles2D DestructionParticles = DestructionParticlesPrefab.Instantiate<GpuParticles2D>();
			DestructionParticles.Position = new(0, Line * 40 - 400 + 20);
			DestructionParticles.Finished += DestructionParticles.QueueFree;
			AddChild(DestructionParticles);
			DestructionParticles.Restart();
		};
	}
}
