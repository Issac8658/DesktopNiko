using Godot;
using System;

public partial class FacepicSelection : Node
{
	enum FacepicType
	{
		Default = 0,
		Speak = 1,
		Scared = 2,
		ScaredSpeak = 3
	}

	[Export]
	public TextureRect PreviewImage;
	[Export]
	public OptionButton FacepicOption;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	
	private void UpdateFacepic()
	{
		
	}
}
