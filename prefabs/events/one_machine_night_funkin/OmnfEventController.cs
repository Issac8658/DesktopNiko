using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using Godot;

public partial class OmnfEventController : Node
{
	public enum Direction { idle, left, down, up, right }
	public const float BEAT_SPEED = 60f / 155f;
	public const float MIN_ACCURACY = 0.1f;
	public const float GAME_SPEED = 1000f;
	public const float HELATH_UP_ON_SUCC = 2.5f;
	public const float HELATH_DOWN_ON_MISS = 5f;

	[Signal]
	public delegate void called_to_clearEventHandler();

	[Export]
	public Window GameWindow;
	[Export]
	public Timer BeatTimer;
	[Export]
	public AudioStreamPlayer Music;
	[Export]
	public AudioStreamPlayer MissSound;
	[Export]
	public AnimationPlayer GlobalAnimator;
	[Export]
	public AnimationPlayer NikoAnimator;
	[Export]
	public AnimationPlayer WMAnimator;
	[Export]
	public AnimationPlayer UIAnimator;
	[Export]
	public PackedScene NikoFlyingArrowsPack;
	[Export]
	public PackedScene WMFlyingArrowsPack;
	[Export]
	public Control AllFlyingArrowsContainer;
	[Export]
	public Control NikoFlyingArrowsContainer;
	[Export]
	public Control WMFlyingArrowsContainer;
	[Export]
	public OMNFVisualController OtherController;
	[Export]
	public string NotesListPath;
	[ExportGroup("Camera Positions", "Camera")]
	[Export]
	public Vector3 CameraDefaultPosition;
	[Export]
	public Vector3 CameraLeftPosition;
	[Export]
	public Vector3 CameraRightPosition;

	public List<ArrowsPack> WMArrows = [];
	public List<ArrowsPack> NikoArrows = [];
	public List<CameraFocusPack> CameraMoves = [];

	private bool IsGamePlaying = false;
	private float GameTime = 0f;

	private int screen_dance_tact = 0;
	public override void _Ready()
	{
		// Screen pulse
		BeatTimer.Connect("timeout", Callable.From(() =>
		{
			if (!NikoAnimator.IsPlaying())
			{
				Dance(NikoAnimator, Direction.idle);
			}
			if (!WMAnimator.IsPlaying())
			{
				Dance(WMAnimator, Direction.idle);
			}
			UIAnimator.Play("health_dance");
			if (screen_dance_tact >= 4)
			{
				UIAnimator.Play("screen_dance");
				screen_dance_tact = 0;
			}
			screen_dance_tact += 1;
		}));
	}
	public override void _Process(double delta)
	{
		if (IsGamePlaying)
		{
			// Game timer and Arrows movement
			GameTime = Music.GetPlaybackPosition();
			AllFlyingArrowsContainer.Position = new Vector2(0f, -GameTime * GAME_SPEED);

			// World Machine arrows auto press
			GetNearestArrow(Direction.idle, WMArrows, out int ArrowId, out bool CanPress);
			ArrowsPack Pack = WMArrows[ArrowId];
			if (CanPress)
			{
				if (Pack.LeftArrow)
				{
					Pack.LeftArrowControl.Visible = false;
					Dance(WMAnimator, Direction.left);
				}
				if (Pack.DownArrow)
				{
					Pack.DownArrowControl.Visible = false;
					Dance(WMAnimator, Direction.down);
				}
				if (Pack.UpArrow)
				{
					Pack.UpArrowControl.Visible = false;
					Dance(WMAnimator, Direction.up);
				}
				if (Pack.RightArrow)
				{
					Pack.RightArrowControl.Visible = false;
					Dance(WMAnimator, Direction.right);
				}
				WMArrows.RemoveAt(ArrowId);
			}
			// Register skiped arrows
			IsArrowSkiped(NikoArrows, out int SkipedArrowId, out bool IsSkiped);
			if (IsSkiped)
			{
				NikoArrows.RemoveAt(SkipedArrowId);
				MissSound.Play();
				OtherController.Health -= HELATH_DOWN_ON_MISS;
			}
			// Camera Move
			CameraCanMove(out int CameraMovePackId, out bool CanMove);
			if (CanMove)
			{
				OtherController.CurrentCameraPosition = CameraMoves[CameraMovePackId].CameraPosition;
				CameraMoves.RemoveAt(CameraMovePackId);
			}
		}
	}

	public override void _Input(InputEvent Event)
	{
		// Keys press register
		if (Event.IsActionPressed("left"))			PressArrow(Direction.left);
		else if (Event.IsActionPressed("down")) 	PressArrow(Direction.down);
		else if (Event.IsActionPressed("up")) 		PressArrow(Direction.up);
		else if (Event.IsActionPressed("right")) 	PressArrow(Direction.right);
	}

	public void DoEvent()
	{

		// loading and creating arrows
		ParseArrows();
		CreateArrows(NikoArrows, NikoFlyingArrowsPack, NikoFlyingArrowsContainer);
		CreateArrows(WMArrows, WMFlyingArrowsPack, WMFlyingArrowsContainer, true);
		UIAnimator.SpeedScale = 1f / BEAT_SPEED;

		// animate window showing
		GlobalAnimator.Play("Show");

		// Starting beat timer, playing music and make Niko invisible
		BeatTimer.WaitTime = BEAT_SPEED;
		BeatTimer.Start();
		Music.Play();
		IsGamePlaying = true;
		GetNode("/root/GlobalControlls").Call("set_niko_visibility", false);
	}

	// Press arrow, if has arrow then remove it, else miss
	private void PressArrow(Direction Dir)
	{
		GetNearestArrow(Dir, NikoArrows, out int ArrowId, out bool CanPress);
		ArrowsPack ArrowsDataPack = NikoArrows[ArrowId];
		if (CanPress)
		{
			Dance(NikoAnimator, Dir);
			switch (Dir)
			{
				case Direction.left:
					ArrowsDataPack.LeftArrowControl.Visible = false;
					break;
				case Direction.down:
					ArrowsDataPack.DownArrowControl.Visible = false;
					break;
				case Direction.up:
					ArrowsDataPack.UpArrowControl.Visible = false;
					break;
				case Direction.right:
					ArrowsDataPack.RightArrowControl.Visible = false;
					break;
			}
			OtherController.Health += HELATH_UP_ON_SUCC;
			NikoArrows.RemoveAt(ArrowId);
		}
		else
		{
			// on miss
			Dance(NikoAnimator, Dir, true);
			MissSound.Play();
			OtherController.Health -= HELATH_DOWN_ON_MISS;
		}
	}
	// animate character
	private static void Dance(AnimationPlayer Animator, Direction Dir, bool Miss = false)
	{
		Animator.SpeedScale = 1;
		Animator.Stop();
		switch (Dir)
		{
			case Direction.idle:
				Animator.SpeedScale = (1f / BEAT_SPEED) + 0.01f;
				Animator.Play("idle");
				break;
			case Direction.left:
				if (Miss) Animator.Play("left_miss");
				else Animator.Play("left");
				break;
			case Direction.down:
				if (Miss) Animator.Play("down_miss");
				else Animator.Play("down");
				break;
			case Direction.up:
				if (Miss) Animator.Play("up_miss");
				else Animator.Play("up");
				break;
			case Direction.right:
				if (Miss) Animator.Play("right_miss");
				else Animator.Play("right");
				break;
		}
	}
	// returns index of nearest arrow below and whether it is possible to press on it 
	private void GetNearestArrow(Direction Arrow, List<ArrowsPack> Arrows, out int ArrowId, out bool CanPress)
	{
		ArrowId = 0;
		CanPress = false;
		for (int i = 0; i < Arrows.Count; i++)
		{
			// for world machine
			if (Arrow == Direction.idle)
			{
				bool HaveAnyArrow = Arrows[i].LeftArrowControl.Visible || Arrows[i].DownArrowControl.Visible
				|| Arrows[i].UpArrowControl.Visible || Arrows[i].RightArrowControl.Visible;

				if (HaveAnyArrow)
				{
					if (Arrows[i].Time <= GameTime)
					{
						ArrowId = i;
						CanPress = true;
						break;
					}
				}
			}
			// for Niko
			else if (PackHasArrow(Arrows[i], Arrow))
			{
				double Time = Arrows[i].Time;
				if (Time >= GameTime - MIN_ACCURACY)
				{
					ArrowId = i;
					if (Mathf.Abs(GameTime - Time) <= MIN_ACCURACY)
					{
						CanPress = true;
					}
					break;
				}
			}
		}
	}
	private void IsArrowSkiped(List<ArrowsPack> Arrows, out int ArrowId, out bool IsSkiped)
	{
		ArrowId = 0;
		IsSkiped = false;
		for (int i = 0; i < Arrows.Count; i++)
		{
			ArrowsPack ArrowsPackData = Arrows[i];
			if (GameTime - MIN_ACCURACY > ArrowsPackData.Time)
			{
				if (ArrowsPackData.LeftArrow || ArrowsPackData.DownArrow || ArrowsPackData.UpArrow || ArrowsPackData.RightArrow)
				{
					ArrowId = i;
					IsSkiped = true;
					break;
				}
			}
			else break;
		}
	}

	private void CameraCanMove(out int CameraMovePackId, out bool CanMove)
	{
		CameraMovePackId = 0;
		CanMove = false;
		for (int i = 0; i < CameraMoves.Count; i++)
		{
			CameraFocusPack Pack = CameraMoves[i];
			if (Pack.Time <= GameTime)
			{
				CameraMovePackId = i;
				CanMove = true;
				break;
			}
		}
	}

	private static bool PackHasArrow(ArrowsPack Pack, Direction dir)
	{
		if (dir == Direction.left && Pack.LeftArrow && Pack.LeftArrowControl.Visible)
		{
			return true;
		}
		else if (dir == Direction.down && Pack.DownArrow && Pack.DownArrowControl.Visible)
		{
			return true;
		}
		else if (dir == Direction.up && Pack.UpArrow && Pack.UpArrowControl.Visible)
		{
			return true;
		}
		else if (dir == Direction.right && Pack.RightArrow && Pack.RightArrowControl.Visible)
		{
			return true;
		}
		return false;
	}

	public struct ArrowsPack(bool Left, bool Down, bool Up, bool Right, double Beat, double Step, byte Opacity = 255)
	{
		public bool LeftArrow = Left;
		public bool DownArrow = Down;
		public bool UpArrow = Up;
		public bool RightArrow = Right;
		
		public double Time = (Beat - 1d + (Step - 1d) / 16d) * BEAT_SPEED * 4d;

		public byte Opacity = Opacity;

		public Control LeftArrowControl;
		public Control DownArrowControl;
		public Control UpArrowControl;
		public Control RightArrowControl;
	}
	public struct CameraFocusPack(Vector3 Position, double Beat, double Step)
	{
		public Vector3 CameraPosition = Position;
		public double Time = (Beat - 1d + (Step - 1d) / 16d) * BEAT_SPEED * 4d;
	}

	public void ParseArrows()
	{
		using var file = FileAccess.Open(NotesListPath, FileAccess.ModeFlags.Read);
		string TextData = file.GetAsText();
		foreach (string ArrowsDataBlock in TextData.Split("\n"))
		{
			string[] SeparatedData = ArrowsDataBlock.Split(";");

			bool[] Arrows = new bool[8];

			double Beat = 1;
			double Step = 1;
			byte Opacity = 255;
			
			bool IsCameraMove = false;
			Vector3 CameraPosition = CameraDefaultPosition;

			if (ArrowsDataBlock.Length > 0)
			{
				for (int i = 0; i < SeparatedData.Length; i++)
				{
					string RawData = SeparatedData[i];
					string Data = RawData.Replace(" ", "");
					if (ArrowsDataBlock[0] == '-' || ArrowsDataBlock[0] == '#')
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
								Beat = double.Parse(Data);
								break;
							case 3:
								//Step
								Step = double.Parse(Data);
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
									CameraPosition = CameraLeftPosition;
								}
								else if (Data[i] == '>')
								{
									CameraPosition = CameraRightPosition;
								}

								break;
							case 1:
								//Beat
								Beat = double.Parse(Data);
								break;
							case 2:
								//Step
								Step = double.Parse(Data);
								break;
						}
					}
				}
			}
			if (IsCameraMove)
			{
				CameraMoves.Add(new CameraFocusPack(CameraPosition, Beat, Step));
			}
			else
			{
				if (Arrows[0] || Arrows[1] || Arrows[2] || Arrows[3])
				{
					WMArrows.Add(new ArrowsPack(Arrows[0], Arrows[1], Arrows[2], Arrows[3], Beat, Step, Opacity));
				}
				if (Arrows[4] || Arrows[5] || Arrows[6] || Arrows[7])
				{
					NikoArrows.Add(new ArrowsPack(Arrows[4], Arrows[5], Arrows[6], Arrows[7], Beat, Step, Opacity));
				}
			}
		}
		GD.Print(string.Format("WM: {0} Niko: {1} Camera: {2}", WMArrows.Count, NikoArrows.Count, CameraMoves.Count));
	}

	public static void CreateArrows(List<ArrowsPack> Arrows, PackedScene PackedArrowsPack, Control Container, bool CompensateAccuracy = false)
	{
		for (int i = 0; i < Arrows.Count; i++)
		{
			ArrowsPack ArrowsDataPack = Arrows[i];
			Control ArrowsObjectsPack = PackedArrowsPack.Instantiate() as Control;

			ArrowsObjectsPack.SetPosition(Vector2.Down * ((float)ArrowsDataPack.Time * GAME_SPEED + MIN_ACCURACY * Convert.ToSingle(CompensateAccuracy)));

			Button LeftArrow = ArrowsObjectsPack.GetNode("left") as Button;
			Button DownArrow = ArrowsObjectsPack.GetNode("down") as Button;
			Button UpArrow = ArrowsObjectsPack.GetNode("up") as Button;
			Button RightArrow = ArrowsObjectsPack.GetNode("right") as Button;

			LeftArrow.Visible = ArrowsDataPack.LeftArrow;
			DownArrow.Visible = ArrowsDataPack.DownArrow;
			UpArrow.Visible = ArrowsDataPack.UpArrow;
			RightArrow.Visible = ArrowsDataPack.RightArrow;
			
			Color Opacity = Color.Color8(255, 255, 255, ArrowsDataPack.Opacity);
			LeftArrow.Modulate *= Opacity;
			DownArrow.Modulate *= Opacity;
			UpArrow.Modulate *= Opacity;
			RightArrow.Modulate *= Opacity;

			ArrowsDataPack.LeftArrowControl = LeftArrow;
			ArrowsDataPack.DownArrowControl = DownArrow;
			ArrowsDataPack.UpArrowControl = UpArrow;
			ArrowsDataPack.RightArrowControl = RightArrow;
			Arrows[i] = ArrowsDataPack;

			Container.AddChild(ArrowsObjectsPack);
		}
	}
}
