using System;
using System.Collections.Generic;
using Godot;
using Omnf;

public partial class OmnfEventController : Node
{
	public enum Direction { idle, left, down, up, right }
	public const float MIN_ACCURACY = 0.1f;
	public const float HELATH_UP_ON_SUCC = 3f;
	public const float HELATH_DOWN_ON_MISS = 5f;

	[Signal]
	public delegate void called_to_clearEventHandler();

	[Export]
	public Window GameWindow;
	[Export]
	public Timer BeatTimer;
	[ExportGroup("Sounds")]
	[Export]
	public AudioStreamPlayer Music;
	[Export]
	public AudioStreamPlayer MissSound;
	[ExportGroup("Animators")]
	[Export]
	public AnimationPlayer GlobalAnimator;
	[Export]
	public AnimationPlayer NikoAnimator;
	[Export]
	public AnimationPlayer WMAnimator;
	[Export]
	public AnimationPlayer UIAnimator;
	[Export]
	public AnimationPlayer HealthAnimator;
	[ExportGroup("Packs")]
	[Export]
	public PackedScene NikoFlyingArrowsPack;
	[Export]
	public PackedScene WMFlyingArrowsPack;
	[Export]
	[ExportGroup("Arrows containers", "ac_")]
	public Control ac_AllFlyingArrowsContainer;
	[Export]
	public Control ac_NikoFlyingArrowsContainer;
	[Export]
	public Control ac_WMFlyingArrowsContainer;
	[Export]
	public OmnfVisualController OtherController;
	[Export]
	public string NotesListPath;
	[ExportGroup("Camera Positions", "Camera")]
	[Export]
	public Vector3 CameraDefaultPosition;
	[Export]
	public Vector3 CameraLeftPosition;
	[Export]
	public Vector3 CameraRightPosition;
	public Vector3[] CameraPositions;

	//Current play status
	private bool IsGamePlaying = false;
	private float GameTime = 0f;
	public int Misses = 0;
	private int screen_dance_tact = 0;

	private SongData Song;
	private float BeatSpeed;
	private float GameSpeed;

	public override void _Ready()
	{
		Song = SongParser.ParseSong(NotesListPath);
		BeatSpeed = Song.BeatSpeed;
		GameSpeed = Song.SongSpeed;
		CameraPositions = [CameraDefaultPosition, CameraLeftPosition, CameraRightPosition];
		// Screen pulse
		UIAnimator.SpeedScale = 1f / BeatSpeed;
		HealthAnimator.SpeedScale = 1f / BeatSpeed + 0.5f;
		BeatTimer.WaitTime = BeatSpeed;
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
			if (screen_dance_tact >= 4)
			{
				UIAnimator.Play("screen_dance");
				screen_dance_tact = 0;
			}
			HealthAnimator.Play("health_dance");
			screen_dance_tact += 1;
		}));
	}
	
	public override void _Process(double delta)
	{
		if (IsGamePlaying)
		{
			// Game timer and Arrows movement
			GameTime = Music.GetPlaybackPosition();
			ac_AllFlyingArrowsContainer.Position = new Vector2(0f, -GameTime * GameSpeed);

			// World Machine arrows auto press
			GetNearestArrow(Direction.idle, Song.LeftCharacterArrows, out int ArrowId, out bool CanPress);
			if (CanPress)
			{
				ArrowsPack Pack = Song.LeftCharacterArrows[ArrowId];
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
				Song.LeftCharacterArrows.RemoveAt(ArrowId);
			}
			// Register skiped arrows
			IsArrowSkiped(Song.RightCharacterArrows, out int SkipedArrowId, out bool IsSkiped);
			if (IsSkiped)
			{
				int MissesOnPack = Convert.ToInt16(Song.RightCharacterArrows[SkipedArrowId].LeftArrow)
				+ Convert.ToInt16(Song.RightCharacterArrows[SkipedArrowId].DownArrow)
				+ Convert.ToInt16(Song.RightCharacterArrows[SkipedArrowId].UpArrow)
				+ Convert.ToInt16(Song.RightCharacterArrows[SkipedArrowId].RightArrow);
				Misses += MissesOnPack;

				Song.RightCharacterArrows.RemoveAt(SkipedArrowId);
				MissSound.Play();
				OtherController.Health -= HELATH_DOWN_ON_MISS;
			}
			// Camera Move
			CameraCanMove(out int CameraMovePackId, out bool CanMove);
			if (CanMove)
			{
				OtherController.CurrentCameraPosition = CameraPositions[Song.CameraMoves[CameraMovePackId].CameraPositionId];
				Song.CameraMoves.RemoveAt(CameraMovePackId);
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
		CreateArrows(Song.RightCharacterArrows, NikoFlyingArrowsPack, ac_NikoFlyingArrowsContainer);
		CreateArrows(Song.LeftCharacterArrows, WMFlyingArrowsPack, ac_WMFlyingArrowsContainer, true);

		// animate window showing
		GlobalAnimator.Play("Show");

		// Starting beat timer, playing music and make Niko invisible
		BeatTimer.Start();
		Music.Stream = Song.Music;
		Music.Play();
		IsGamePlaying = true;
		GetNode("/root/GlobalControlls").Call("set_niko_visibility", false);
	}

	// Press arrow, if has arrow then remove it, else miss
	private void PressArrow(Direction Dir)
	{
		GetNearestArrow(Dir, Song.RightCharacterArrows, out int ArrowId, out bool CanPress);
		if (CanPress)
		{
			ArrowsPack ArrowsDataPack = Song.RightCharacterArrows[ArrowId];
			Dance(NikoAnimator, Dir);
			switch (Dir)
			{
				case Direction.left:
					ArrowsDataPack.LeftArrowControl.Visible = false;
					ArrowsDataPack.LeftArrow = false;
					break;
				case Direction.down:
					ArrowsDataPack.DownArrowControl.Visible = false;
					ArrowsDataPack.DownArrow = false;
					break;
				case Direction.up:
					ArrowsDataPack.UpArrowControl.Visible = false;
					ArrowsDataPack.UpArrow = false;
					break;
				case Direction.right:
					ArrowsDataPack.RightArrowControl.Visible = false;
					ArrowsDataPack.RightArrow = false;
					break;
			}
			OtherController.Health += HELATH_UP_ON_SUCC;
			Song.RightCharacterArrows[ArrowId] = ArrowsDataPack;
			if (!(ArrowsDataPack.LeftArrow || ArrowsDataPack.DownArrow || ArrowsDataPack.UpArrow || ArrowsDataPack.RightArrow))
				Song.RightCharacterArrows.RemoveAt(ArrowId);
		}
		else
		{
			// on miss
			Dance(NikoAnimator, Dir, true);
			MissSound.Play();
			Misses += 1;
			OtherController.Health -= HELATH_DOWN_ON_MISS;
		}
	}
	// animate character
	private void Dance(AnimationPlayer Animator, Direction Dir, bool Miss = false)
	{
		Animator.SpeedScale = 1;
		Animator.Stop();
		switch (Dir)
		{
			case Direction.idle:
				Animator.SpeedScale = (1f / BeatSpeed) + 0.01f;
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
		for (int i = 0; i < Song.CameraMoves.Count; i++)
		{
			CameraMovePack Pack = Song.CameraMoves[i];
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

	public void CreateArrows(List<ArrowsPack> Arrows, PackedScene PackedArrowsPack, Control Container, bool CompensateAccuracy = false)
	{
		for (int i = 0; i < Arrows.Count; i++)
		{
			ArrowsPack ArrowsDataPack = Arrows[i];
			Control ArrowsObjectsPack = PackedArrowsPack.Instantiate() as Control;

			ArrowsObjectsPack.SetPosition(Vector2.Down * ((float)ArrowsDataPack.Time * GameSpeed + MIN_ACCURACY * Convert.ToSingle(CompensateAccuracy)));

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
