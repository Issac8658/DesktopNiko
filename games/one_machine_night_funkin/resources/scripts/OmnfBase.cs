using Godot;
using System;
using System.Collections.Generic;

namespace Omnf
{
	public struct ArrowsPack(bool Left, bool Down, bool Up, bool Right, float Beat, float Step, float BeatSpeed, byte Opacity = 255)
	{
		public bool LeftArrow = Left;
		public bool DownArrow = Down;
		public bool UpArrow = Up;
		public bool RightArrow = Right;

		public float Time = (Beat - 1f + (Step - 1f) / 16f) * BeatSpeed * 4f;

		public byte Opacity = Opacity;

		public Control LeftArrowControl;
		public Control DownArrowControl;
		public Control UpArrowControl;
		public Control RightArrowControl;
	}

	public struct CameraMovePack(int CameraPosition, float Beat, float Step, float BeatSpeed)
	{
		public int CameraPositionId = CameraPosition;
		public float Time = SongParser.BeatToTime(Beat, Step, BeatSpeed);
	}

	public struct SongData()
	{
		public float Tempo = 120;
		public float BeatSpeed
		{
			set => Tempo = 60f / value;
			get => 60f / Tempo;
		}
		public float SongSpeed = 800;
		public AudioStream Music;
		public List<float[]> Bumps = [];
		public List<ArrowsPack> LeftCharacterArrows = [];
		public List<ArrowsPack> RightCharacterArrows = [];
		public List<CameraMovePack> CameraMoves = [];

	}

	public static class SongParser
	{
		public static float BeatToTime(float Beat, float Step, float BeatSpeed)
		{
			return (Beat - 1f + (Step - 1f) / 16f) * BeatSpeed * 4f;
		}
		public static SongData ParseSong(string NotesListPath)
		{
			SongData Song = new();

			var file = FileAccess.Open(NotesListPath, FileAccess.ModeFlags.Read);
			string[] TextData = file.GetAsText().Replace(" ", "").Split("\n");
			for (int TextDataIndex = 0; TextDataIndex < TextData.Length; TextDataIndex++) // for each line
			{
				string DataBlock = TextData[TextDataIndex]; // full line
				string[] SeparatedData = DataBlock.Split(";"); // separated line by ;

				switch (TextDataIndex)
				{
					case 0:
						GD.Print(DataBlock);
						Song.Music = AudioStreamMP3.LoadFromFile(DataBlock.Trim());
						break;
					case 1:
						Song.Tempo = float.Parse(DataBlock);
						break;
					case 2:
						Song.SongSpeed = float.Parse(DataBlock);
						break;
					case 3:
						foreach (string BumpPart in SeparatedData)
						{
							string[] Positions = BumpPart.Split("-"); // like 1:1 and 2:2
							string[] Pos1 = Positions[0].Split(":"); // like 1 and 1
							string[] Pos2 = Positions[1].Split(":"); // like 2 and 2

							Song.Bumps.Add([
								BeatToTime(float.Parse(Pos1[0]), float.Parse(Pos1[1]), Song.BeatSpeed),
								BeatToTime(float.Parse(Pos2[0]), float.Parse(Pos2[1]), Song.BeatSpeed)
							]);
						}
						break;
				}

				if (TextDataIndex > 3)
				{
					if (DataBlock.Length > 0)
					{
						bool[] Arrows = new bool[8];

						float Beat = 1;
						float Step = 1;
						byte Opacity = 255;
						bool IsCameraMove = false;
						int CameraPosition = 0;
						
						for (int i = 0; i < SeparatedData.Length; i++) // for each data IN line
						{
							string Data = SeparatedData[i];
							if (DataBlock[0] == '-' || DataBlock[0] == '#')
							{
								switch (i)
								{
									case 0:
										// Arrows
										for (int ArrowIndex = 0; ArrowIndex < Data.Length; ArrowIndex++)
										{
											if (Data[ArrowIndex] == '#') Arrows[ArrowIndex] = true;
										}
										break;
									case 1:
										// Opacity
										Opacity = byte.Parse(Data);
										break;
									case 2:
										//Beat
										Beat = float.Parse(Data);
										break;
									case 3:
										//Step
										Step = float.Parse(Data);
										break;
								}
							}
							else
							{
								IsCameraMove = true;
								switch (i)
								{
									case 0:
										// type
										if (Data[i] == '<')
										{
											CameraPosition = 1;
										}
										else if (Data[i] == '>')
										{
											CameraPosition = 2;
										}

										break;
									case 1:
										//Beat
										Beat = float.Parse(Data);
										break;
									case 2:
										//Step
										Step = float.Parse(Data);
										break;
								}
							}
						}
						if (IsCameraMove)
						{
							Song.CameraMoves.Add(new CameraMovePack(CameraPosition, Beat, Step, Song.BeatSpeed));
						}
						else
						{
							if (Arrows[0] || Arrows[1] || Arrows[2] || Arrows[3])
							{
								Song.LeftCharacterArrows.Add(new ArrowsPack(Arrows[0], Arrows[1], Arrows[2], Arrows[3], Beat, Step, Song.BeatSpeed, Opacity));
							}
							if (Arrows[4] || Arrows[5] || Arrows[6] || Arrows[7])
							{
								Song.RightCharacterArrows.Add(new ArrowsPack(Arrows[4], Arrows[5], Arrows[6], Arrows[7], Beat, Step, Song.BeatSpeed, Opacity));
							}
						}
					}
				}
			}

			GD.Print(string.Format("WM: {0} Niko: {1} Camera: {2}", Song.LeftCharacterArrows.Count, Song.RightCharacterArrows.Count, Song.CameraMoves.Count));
			return Song;
		}
	}
}
