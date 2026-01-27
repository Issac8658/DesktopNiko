using Godot;
using System;

public partial class TetrisProgression : Node
{
	[Export]
	public AnimationPlayer AnimationPlayer;

	public void StartProgression()
	{
		AnimationPlayer.Play("Progression");
	}
}
