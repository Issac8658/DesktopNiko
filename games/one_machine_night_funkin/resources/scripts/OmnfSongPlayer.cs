using Godot;
using System;
using System.Collections.Generic;

public partial class OmnfSongPlayer : AudioStreamPlayer
{
	[Export]
	public string PathToSongList;
	[Signal]
	public delegate void BeatEventHandler();
	public int CurrentBeat = 0;
	public Song CurrentSong;
	public double BeatTime = 0.5d;
	private int LastBeat = -1;

	public List<Song> SongList = [];

	public override void _Ready()
	{
		ParseSongs();
	}

	public bool PlaySong(string SongId) // return false if unknown song id
	{
		CurrentSong = FindSong(SongId);
		if (CurrentSong.Id != "EMPTY")
		{
			CurrentBeat = 0;
			Stream = CurrentSong.AudioStreamResource; // null reference expention
			BeatTime = 60d / CurrentSong.Tempo;
			Play();
			return true;
		}
		GD.PushWarning("Unknown song \"" + SongId + "\"! Playing void.");
		return false;
	}

	public override void _Process(double delta)
	{
		if (Playing)
		{
			CurrentBeat = (int)Mathf.Floor(GetPlaybackPosition() / BeatTime);
			if (LastBeat != CurrentBeat)
			{
				LastBeat = CurrentBeat;
				EmitSignal("Beat");
			}
		}
	}


	public void StopSong()
	{
		Stop();
		CurrentBeat = 0;
	}

	public Song FindSong(string SongId)
	{
		for (int i = 0; i < SongList.Count; i++)
		{
			if (SongList[i].Id == SongId)
			{
				return SongList[i];
			}
		}
		return new Song("EMPTY");
	}
	public struct Song(string SongId, AudioStream SongAudioStreamResource = null, float SongTempo = 120f)
	{
		public string Id = SongId;
		public AudioStream AudioStreamResource = SongAudioStreamResource;
		public float Tempo = SongTempo;
	}

	private void ParseSongs()
	{
		using var file = FileAccess.Open(PathToSongList, FileAccess.ModeFlags.Read);
		string[] TextData = file.GetAsText().Replace(" ", "").Split("\n");
		foreach (string Line in TextData)
		{
			string TrimmedLine = Line.Trim();
			if (TrimmedLine != "")
			{
				string[] SongData = TrimmedLine.Split(";");
				AudioStream stream = GD.Load<AudioStream>(SongData[1]);
				//GD.Print(stream); // has audiostream
				SongList.Add(new Song(
					SongData[0],
					stream, // null reference exception
					float.Parse(SongData[2])
				));
			}
		}
	}
}
