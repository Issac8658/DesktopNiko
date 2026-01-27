using Godot;
using System;

public partial class JsonMidiPlayer : Node
{
	[Signal] public delegate void MidiPressedEventHandler(int Midi, string Channel, float time, float duration);

	[Export] public AudioStreamPlayer AttachedPlayer;
	[Export] public string MidiPath;
	
	private float _oldTime = 0;

	private Variant PackedMidi;

	public override void _Ready()
	{
		using FileAccess file = FileAccess.Open(MidiPath, FileAccess.ModeFlags.Read);
		string rawText = file.GetAsText();
		file.Close();

		Json json = new();
		Error error = json.Parse(rawText);
		if(error == Error.Ok)
		{
			PackedMidi = json.Data;
		}
		else
			GD.Print("JSON Parse Error: ", json.GetErrorMessage(), " in ", MidiPath, " at line ", json.GetErrorLine());

		//GD.Print(PackedMidi);
	}


	public override void _Process(double delta)
	{
		float time = AttachedPlayer.GetPlaybackPosition();

		_oldTime = time;
	}
}
