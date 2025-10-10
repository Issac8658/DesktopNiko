using Godot;
using System;
using System.IO;

public partial class StoryModeMenuSelector : Node
{
	public const string WEEKS_PATH = "res://games/one_machine_night_funkin/data/weeks/";
	[Export]
	public Container WeeksContainer;
	[Export]
	public Container TracksContainer;
	[Export]
	public Label PointsLabel;
	[Export]
	public Label WeekTitleLabel;
	[Export]
	public TextureRect WeekBG;
	[Export]
	public PackedScene WeekLabelTemplate;
	[Export]
	public PackedScene TrackLabelTemplate;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ParseWeeks();
	}

	public struct Week(string Id, string Title, string Desc, Texture2D Bg, string[] SongsIds, string[] Difficulties)
	{
		public string WeekId = Id;
		public string WeekTitle = Title;
		public string WeekDesc = Desc;
		public Texture2D WeekBg = Bg;
		public string[] WeekSongsIds = SongsIds;
		public string[] WeekDifficulties = Difficulties;
	}

	public void ParseWeeks()
	{
		var Dir = DirAccess.Open(WEEKS_PATH);
		if (Dir == null)
		{
			GD.PrintErr("Invalid weeks path");
			return;
		}
		foreach (string WeekFolder in Dir.GetFiles())
		{
			GD.Print(WeekFolder);
		}
	}
}
