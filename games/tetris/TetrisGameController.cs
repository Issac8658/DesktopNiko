using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tetris
{
	public partial class TetrisGameController : Window
	{

		public const int GAME_HEIGHT = 20;
		public const int GAME_WIDTH = 10;
		public const double START_TICK_TIME = 0.5;
		public const int BASE_WINDOW_HEIGHT = 1080;

		// signals
		[Signal]
		public delegate void ReDrawTilesEventHandler();
		[Signal]
		public delegate void ReDrawNextTilesEventHandler();
		[Signal]
		public delegate void ReDrawZoneEventHandler(Rect2I Zone);
		[Signal]
		public delegate void StateChangedEventHandler();
		[Signal]
		public delegate void LineDestroyedEventHandler(short Line);
		[Signal]
		public delegate void BlockDroppedEventHandler(int DestroyedLinesCount, SpinType Spin);
		[Signal]
		public delegate void ComboChangedEventHandler(uint Combo);
		[Signal]
		public delegate void RestartedEventHandler();

		// enums
		public enum GameStates
		{
			Menu,
			Playing,
			Restart,
			Paused,
			Lose
		}

		public enum TetrisAction
		{
			Unknown,
			Move,
			Rotation
		}

		public enum SpinType
		{
			None,
			TSpin,
			TSpinMini,
			AllSpin
		}

		public static readonly AudioEffectLowPassFilter LowPassFilter = AudioServer.GetBusEffect(2, 0) as AudioEffectLowPassFilter;

		private List<List<Figure.FigureColor?>> _blocks = [];
		private List<Figure> _nextFigures = [];
		private Figure _currentFigure;

		private uint _score = 0;
		private uint _lines = 0;
		private uint _combo = 0;
		private TetrisAction _lastAction = TetrisAction.Unknown;
		private SpinType _currentSpin = SpinType.None;
		private GameStates _currentState = GameStates.Menu;
		private short _rotationAttempt = -1;

		public uint Combo
		{
			get => _combo;
			set
			{
				_combo = value;
				EmitSignal("ComboChanged", value);
			}
		}

		public GameStates CurrentState
		{
			get => _currentState;
			set
			{
				_currentState = value;
				EmitSignal("StateChanged");
			}
		}

		public Figure CurrentFigure
		{
			get => _currentFigure;
		}
		public Figure[] NextFigures
		{
			get => [.. _nextFigures];
		}
		public List<List<Figure.FigureColor?>> Blocks { get => _blocks; }

		[Export]
		public Timer GameTimer;

		[ExportGroup("Other shit")]
		[Export]
		public Label FpsCounter;
		[Export]
		public Label ScoreCounter;
		[Export]
		public Label LinesCounter;
		[Export]
		public Control LeftRuler;
		[Export]
		public Control RightRuler;
		[Export]
		public Control GlobalAnchor;

		[ExportGroup("Buttons")]
		[Export]
		public Button PauseButton;
		[Export]
		public Button RestartButton;
		[Export]
		public Button ExitButton;
		[ExportGroup("Sounds")]
		[Export]
		public AudioStreamPlayer PlaceLightSound;
		[Export]
		public AudioStreamPlayer PlaceNormalSound;
		[Export]
		public AudioStreamPlayer PlaceMegaSound;
		[Export]
		public AudioStreamPlayer PlaceSound;
		[Export]
		public AudioStreamPlayer HeightPlaceSound;


		public override void _Ready()
		{
			for (int Y = 0; Y < GAME_HEIGHT; Y++) // init map
			{
				List<Figure.FigureColor?> Line = [];
				for (int X = 0; X < GAME_HEIGHT; X++)
					Line.Add(null);
				_blocks.Add(Line);
			}

			for (int i = 0; i < 4; i++) // filling next figures
				_nextFigures.Add(new());

			_currentFigure = new(); // creating current figure

			GameTimer.WaitTime = START_TICK_TIME;
			GameTimer.Timeout += Tick; // ticks timer

			StateChanged += () => // states
			{
				switch (CurrentState)
				{
					case GameStates.Restart:
						RestartGame();
						CurrentState = GameStates.Playing;
						TweenLowPass(20500);
						break;
					case GameStates.Playing:
						GameTimer.Start();
						TweenLowPass(20500);
						break;
					default:
						GameTimer.Stop();
						break;
				}
			};

			// buttons

			RestartButton.Pressed += RestartGame; // restart
			PauseButton.Pressed += () => // pause
			{
				if (CurrentState == GameStates.Playing)
				{
					CurrentState = GameStates.Paused;
					TweenLowPass(300);
				}
				else if (CurrentState == GameStates.Paused)
				{
					CurrentState = GameStates.Playing;
					TweenLowPass(20500);
				}
			};
			ExitButton.Pressed += () =>
			{
				CurrentState = GameStates.Menu;
				TweenLowPass(300);
			}; // menu

			BlockDropped += (DestroyedLinesCount, Spin) => _score += SpinPoints[Spin][DestroyedLinesCount];

			// init redraw
			ReDraw();
			UpdateRulers();

			int CurrentScreen = DisplayServer.WindowGetCurrentScreen(GetWindowId());

			Size = DisplayServer.ScreenGetSize(CurrentScreen) - new Vector2I(20, 0);
			Position = DisplayServer.ScreenGetPosition(CurrentScreen) + new Vector2I(10, 0);
			GlobalAnchor.Scale = new Vector2((float)Size.Y / BASE_WINDOW_HEIGHT, (float)Size.Y / BASE_WINDOW_HEIGHT);
		}

		public override void _Process(double delta)
		{
			FpsCounter.Text = "FPS: " + (1.0 / delta).ToString(); // just fps
		}

		public override void _Input(InputEvent @event)
		{
			if (CurrentState == GameStates.Playing)
			{
				if (@event.IsActionPressed("left")) // a, ←
				{
					MoveCurrentFigure(new(-1, 0)); // move current figure left for 1 block
					ReDrawCurrentFigure();
				}
				else if (@event.IsActionPressed("right")) // d, →
				{
					MoveCurrentFigure(new(1, 0)); // move current figure right for 1 block
					ReDrawCurrentFigure();
				}
				else if (@event.IsActionPressed("up")) // w, ↑
				{
					RotateCurrentFigure(); // rotate current figure
					ReDrawCurrentFigure();
				}
				else if (@event.IsActionPressed("rotate_left")) // q
				{
					RotateCurrentFigure(true); // rotate current figure
					ReDrawCurrentFigure();
				}
				else if (@event.IsActionPressed("rotate_right")) // e
				{
					RotateCurrentFigure(); // rotate current figure
					ReDrawCurrentFigure();
				}
				else if (@event.IsActionPressed("down")) // s, ↓
				{
					TickCurrentFigure(); // move current figure down for 1 block
					ReDrawCurrentFigure();
				}
				else if (@event.IsActionPressed("dialog_skip")) // space, enter, z
				{
					while (!TickCurrentFigure(true)) ; // move current figure to bottom
					UpdateStats();
				}
			}
			// clear map for tests
			/*else if (@event.IsActionPressed("back")) // esc, x, backspace, delete
			{
				_blocks = [];
				for (int Y = 0; Y < GAME_HEIGHT; Y++)
				{
					List<BlockType?> Line = [];
					for (int X = 0; X < GAME_HEIGHT; X++)
					{
						Line.Add(null);
					}
					_blocks.Add(Line);
				}
			}*/
		}

		private void Tick() // calls every timer timeout
		{
			TickCurrentFigure(); // figure physics
			UpdateStats(); // stats update

			if (!AchievementsController.IsAchievementTakedStatic("tetris_25000") && _score >= 25000)
				AchievementsController.TakeAchievementStatic("tetris_25000");
		}

		private void ReDrawCurrentFigure(bool Drop = false) // redrawing current figure
		{
			if (Drop)
				EmitSignal("ReDrawZone", new Rect2I(_currentFigure.Position.X - 2, 0, 4 + _currentFigure.Size.X, GAME_HEIGHT)); // full colums if pressing space
			else
				EmitSignal("ReDrawZone", new Rect2I(_currentFigure.Position - new Vector2I(3, 3), new Vector2I(6, 6) + _currentFigure.Size)); // else zone around figure

			UpdateRulers();
		}

		private void UpdateRulers()
		{
			Vector2I ActiveZonePosition = _currentFigure.Position + _currentFigure.ActiveZone.Position;

			LeftRuler.Position = new(ActiveZonePosition.X * 40 - 3, 0); // positing left ruler
			RightRuler.Position = LeftRuler.Position + new Vector2(_currentFigure.ActiveZone.Size.X * 40, 0); // positing right ruler
			RightRuler.Modulate = LeftRuler.Modulate = _currentFigure.GetColor();
		}

		private void ReDraw()
		{
			EmitSignal("ReDrawTiles"); // call for TetrisTileMap.cs
		}

		private void UpdateStats()
		{
			ScoreCounter.Text = _score.ToString();
			LinesCounter.Text = _lines.ToString();
		}

		private bool TickCurrentFigure(bool HeightPlace = false) // returns whether the figure touched ground or another figure
		{
			if (FigureCanMove(_currentFigure, Vector2I.Down)) // checking if current figure can move
			{
				_currentFigure.Position.Y += 1; // moving
				_score += 1;
				ReDrawCurrentFigure();
				return false;
			}
			else
			{
				// redrawing and figure place and some similar shit idk
				PlaceCurrentFigure();
				_currentFigure = _nextFigures[0];
				_nextFigures.RemoveAt(0);
				_nextFigures.Add(new());
				_score += 10;


				// LinesChecking
				uint DestroyedLines = 0;
				for (int Y = 0; Y < GAME_HEIGHT; Y++)
				{
					bool lineFull = false;
					for (int X = 0; X < GAME_WIDTH; X++)
						if (_blocks[X][Y] != null)
							lineFull = true;
						else
						{
							lineFull = false;
							break;
						}
					if (lineFull)
					{
						for (int X = 0; X < GAME_WIDTH; X++)
						{
							_blocks[X].RemoveAt(Y);
							_blocks[X].Insert(0, null);
						}
						DestroyedLines++;
						EmitSignal("LineDestroyed", Y);

						GameTimer.WaitTime *= 0.995;
					}
				}

				if (DestroyedLines > 0)
				{
					ReDraw();
					Combo += 1;
				}
				else
					Combo = 0;

				// sounds and score if some lines destroyed
				_score += DestroyedLinesScores[DestroyedLines] * Combo;
				switch (DestroyedLines)
				{
					case 0:
						if (HeightPlace)
							HeightPlaceSound.Play();
						else
							PlaceSound.Play();
						break;
					case 1: PlaceLightSound.Play(); break;
					case 2: PlaceNormalSound.Play(); break;
					case 3: PlaceMegaSound.Play(); break;
					case 4:
					{
						PlaceMegaSound.Play();
						if (!AchievementsController.IsAchievementTakedStatic("tetris"))
							AchievementsController.TakeAchievementStatic("tetris");
						break;
					}
				}
				_lines += DestroyedLines;

				if (_lastAction == TetrisAction.Rotation)
					switch (_currentFigure.Type)
					{
						case Figure.FigureType.T:
							{
								if (_rotationAttempt == WallKickTable4x4[0].Length - 1)
									EmitSignal("BlockDropped", DestroyedLines, (int)SpinType.TSpin);
								else
									switch (_currentFigure.Rotation)
									{
										case Figure.ROTATION_0:
											{
												if (_blocks[_currentFigure.Position.X][_currentFigure.Position.Y] != null
												 && _blocks[_currentFigure.Position.X + 2][_currentFigure.Position.Y] != null
												 &&(_blocks[_currentFigure.Position.X][_currentFigure.Position.Y + 2] != null
												 || _blocks[_currentFigure.Position.X + 2][_currentFigure.Position.Y + 2] != null))
													EmitSignal("BlockDropped", DestroyedLines, (int)SpinType.TSpin);
												else if ((_blocks[_currentFigure.Position.X][_currentFigure.Position.Y] != null
													   || _blocks[_currentFigure.Position.X + 2][_currentFigure.Position.Y] != null)
													   && _blocks[_currentFigure.Position.X][_currentFigure.Position.Y + 2] != null
													   && _blocks[_currentFigure.Position.X + 2][_currentFigure.Position.Y + 2] != null)
													EmitSignal("BlockDropped", DestroyedLines, (int)SpinType.TSpinMini);
												else
													EmitSignal("BlockDropped", DestroyedLines, (int)SpinType.None);
												break;
											}
										case Figure.ROTATION_R:
											{
												if (_blocks[_currentFigure.Position.X + 2][_currentFigure.Position.Y] != null
												 && _blocks[_currentFigure.Position.X + 2][_currentFigure.Position.Y + 2] != null
												 &&(_blocks[_currentFigure.Position.X][_currentFigure.Position.Y] != null
												 || _blocks[_currentFigure.Position.X][_currentFigure.Position.Y + 2] != null))
													EmitSignal("BlockDropped", DestroyedLines, (int)SpinType.TSpin);
												else if ((_blocks[_currentFigure.Position.X + 2][_currentFigure.Position.Y] != null
													   || _blocks[_currentFigure.Position.X + 2][_currentFigure.Position.Y + 2] != null)
													   && _blocks[_currentFigure.Position.X][_currentFigure.Position.Y] != null
													   && _blocks[_currentFigure.Position.X][_currentFigure.Position.Y + 1] != null)
													EmitSignal("BlockDropped", DestroyedLines, (int)SpinType.TSpinMini);
												else
													EmitSignal("BlockDropped", DestroyedLines, (int)SpinType.None);
												break;
											}
										case Figure.ROTATION_2:
											{
												if (_blocks[_currentFigure.Position.X + 2][_currentFigure.Position.Y + 2] != null
												 && _blocks[_currentFigure.Position.X][_currentFigure.Position.Y + 2] != null
												 &&(_blocks[_currentFigure.Position.X + 2][_currentFigure.Position.Y] != null
												 || _blocks[_currentFigure.Position.X][_currentFigure.Position.Y] != null))
													EmitSignal("BlockDropped", DestroyedLines, (int)SpinType.TSpin);
												else if ((_blocks[_currentFigure.Position.X + 2][_currentFigure.Position.Y + 2] != null
													   || _blocks[_currentFigure.Position.X][_currentFigure.Position.Y + 2] != null)
													   && _blocks[_currentFigure.Position.X + 2][_currentFigure.Position.Y] != null
													   && _blocks[_currentFigure.Position.X][_currentFigure.Position.Y] != null)
													EmitSignal("BlockDropped", DestroyedLines, (int)SpinType.TSpinMini);
												else
													EmitSignal("BlockDropped", DestroyedLines, (int)SpinType.None);
												break;
											}
										case Figure.ROTATION_L:
											{
												if (_blocks[_currentFigure.Position.X][_currentFigure.Position.Y + 2] != null
												 && _blocks[_currentFigure.Position.X][_currentFigure.Position.Y] != null
												 &&(_blocks[_currentFigure.Position.X + 2][_currentFigure.Position.Y + 2] != null
												 || _blocks[_currentFigure.Position.X + 2][_currentFigure.Position.Y] != null))
													EmitSignal("BlockDropped", DestroyedLines, (int)SpinType.TSpin);
												else if ((_blocks[_currentFigure.Position.X][_currentFigure.Position.Y + 2] != null
													   || _blocks[_currentFigure.Position.X][_currentFigure.Position.Y] != null)
													   && _blocks[_currentFigure.Position.X + 2][_currentFigure.Position.Y + 2] != null
													   && _blocks[_currentFigure.Position.X + 2][_currentFigure.Position.Y] != null)
													EmitSignal("BlockDropped", DestroyedLines, (int)SpinType.TSpinMini);
												else
													EmitSignal("BlockDropped", DestroyedLines, (int)SpinType.None);
												break;
											}
										default:
											{
												EmitSignal("BlockDropped", DestroyedLines, (int)SpinType.None);
												break;
											}
									}
								break;
							}
						default:
							{
								if (!FigureCanMove(_currentFigure, Vector2I.Up)
								 && !FigureCanMove(_currentFigure, Vector2I.Right)
								 && !FigureCanMove(_currentFigure, Vector2I.Left)
								 && !FigureCanMove(_currentFigure, Vector2I.Down))
									EmitSignal("BlockDropped", DestroyedLines, (int)SpinType.AllSpin);
								else
									EmitSignal("BlockDropped", DestroyedLines, (int)SpinType.None);
								break;
							}
					}
				else
					EmitSignal("BlockDropped", DestroyedLines, (int)SpinType.None);

				if (!FigureCanMove(_nextFigures[0], Vector2I.Zero)) // lose checking
				{
					CurrentState = GameStates.Lose;
					return true;
				}
				_lastAction = TetrisAction.Unknown;

				EmitSignal("ReDrawNextTiles");
				ReDrawCurrentFigure(true);

				return true;
			}
		}

		private void RestartGame() // full restart
		{
			_blocks = [];
			for (int Y = 0; Y < GAME_HEIGHT; Y++)
			{
				List<Figure.FigureColor?> Line = [];
				for (int X = 0; X < GAME_HEIGHT; X++)
					Line.Add(null);
				_blocks.Add(Line);
			}
			_nextFigures = [];
			for (int i = 0; i < 4; i++)
				_nextFigures.Add(new());

			_currentFigure = new();
			_score = 0;
			_lines = 0;
			GameTimer.WaitTime = START_TICK_TIME;
			ReDraw();
			UpdateRulers();
			EmitSignal("ReDrawNextTiles");
			EmitSignal("Restarted");
		}

		private void MoveCurrentFigure(Vector2I Offset)
		{
			if (FigureCanMove(_currentFigure, Offset))
			{
				_currentFigure.Position += Offset;
				_lastAction = TetrisAction.Move;
				_rotationAttempt = -1;
			}
		}

		private bool FigureCanMove(Figure Figure, Vector2I Offset)
		{
			Vector2I ActiveZonePosition = Figure.Position + Figure.ActiveZone.Position + Offset;

			if (ActiveZonePosition.X < 0
			 || ActiveZonePosition.X + Figure.ActiveZone.Size.X - 1 >= GAME_WIDTH
			 || ActiveZonePosition.Y + Figure.ActiveZone.Size.Y - 1 >= GAME_HEIGHT)
				return false;

			for (int X = Figure.ActiveZone.Position.X; X < Figure.ActiveZone.Position.X + Figure.ActiveZone.Size.X; X++)
				for (int Y = Figure.ActiveZone.Position.Y; Y < Figure.ActiveZone.Position.Y + Figure.ActiveZone.Size.Y; Y++)
					if (Figure[X][Y])
					{
						Vector2I globalPos = new Vector2I(X, Y) + Figure.Position + Offset;
						if (globalPos.Y >= 0 && _blocks[globalPos.X][globalPos.Y] != null)
							return false;
					}
			return true;
		}
		private bool FigureCanMove(bool[][] Form, Vector2I Position)
		{
			Rect2I ActiveZone = Figure.GetActiveZone(Form);
			Vector2I ActiveZonePosition = Position + ActiveZone.Position;

			if (ActiveZonePosition.X < 0
			 || ActiveZonePosition.X + ActiveZone.Size.X - 1 >= GAME_WIDTH
			 || ActiveZonePosition.Y + ActiveZone.Size.Y - 1 >= GAME_HEIGHT)
				return false;

			for (int X = ActiveZone.Position.X; X < ActiveZone.Position.X + ActiveZone.Size.X; X++)
				for (int Y = ActiveZone.Position.Y; Y < ActiveZone.Position.Y + ActiveZone.Size.Y; Y++)
					if (Form[X][Y])
					{
						Vector2I globalPos = new Vector2I(X, Y) + Position;
						if (globalPos.Y >= 0 && _blocks[globalPos.X][globalPos.Y] != null)
							return false;
					}
			return true;
		}

		private void MoveCurrentFigureInBorders() // if figure want to go out of bounds
		{
			int ActiveZonePosition = _currentFigure.Position.X + _currentFigure.ActiveZone.Position.X;
			_currentFigure.Position.X = Mathf.Clamp(ActiveZonePosition, 0, GAME_WIDTH - _currentFigure.ActiveZone.Size.X + 1) - _currentFigure.ActiveZone.Position.X;
		}

		private void RotateCurrentFigure(bool Reverse = false) // figure rotating
		{
			if (_currentFigure.Length == 2) return;

			bool[][] RotatedFigureArray = _currentFigure.GetRotatedForm(Reverse);
			// getting offsets table
			Vector2I[][] Table = RotatedFigureArray.Length == 3 ? WallKickTable3x3 : WallKickTable4x4;

			// SRS
			int NextRotation = Reverse ?
			_currentFigure.Rotation == 0 ? Figure.ROTATION_L : _currentFigure.Rotation - 1 :
			_currentFigure.Rotation + 1 > Figure.ROTATION_L ? Figure.ROTATION_0 : _currentFigure.Rotation + 1;

			for (short attempt = 0; attempt <= Table[0].Length; attempt++)
			{
				Vector2I ResultPosition = _currentFigure.Position + GetWallKickOffset(Table, _currentFigure.Rotation, NextRotation, attempt);
				if (FigureCanMove(RotatedFigureArray, ResultPosition))
				{
					_currentFigure.Rotate(Reverse);
					_currentFigure.Position = ResultPosition;
					//MoveCurrentFigureInBorders();
					_rotationAttempt = attempt;
					_lastAction = TetrisAction.Rotation;
					return;
				}
			}
		}

		private void PlaceCurrentFigure()
		{
			for (int X = 0; X < GAME_WIDTH; X++)
				for (int Y = 0; Y < GAME_HEIGHT; Y++)
					if (CurrentFigureIsHere(new(X, Y)))
						_blocks[X][Y] = _currentFigure.Color;
		}

		public bool CurrentFigureIsHere(Vector2I Position) // checking if current figure's block in this pos 
		{
			if (Position.X - _currentFigure.Position.X >= 0 && Position.X - _currentFigure.Position.X < _currentFigure.Size.X &&
				Position.Y - _currentFigure.Position.Y >= 0 && Position.Y - _currentFigure.Position.Y < _currentFigure.Size.Y) // wtf is this that..?
				return _currentFigure[Position.X - _currentFigure.Position.X][Position.Y - _currentFigure.Position.Y];

			return false;
		}

		private void TweenLowPass(float result)
		{
			Tween tween = CreateTween();
			tween.SetTrans(Tween.TransitionType.Quint);
			tween.TweenProperty(LowPassFilter, "cutoff_hz", result, 0.25);
		}

		public Vector2I GetLowestFigurePoint(Figure Figure, Vector2I Position)
		{
			while (true)
			{
				bool CanMove = true;
				for (int X = 0; X < Figure.Length; X++)
					for (int Y = 0; Y < Figure[0].Length; Y++)
						if (Figure[X][Y])
							CanMove &= Position.Y + Y + 1 < GAME_HEIGHT && Blocks[Position.X + X][Position.Y + Y + 1] == null;
				if (!CanMove) return Position;
				Position += Vector2I.Down;
			}
		}

		public static readonly Vector2I[][] WallKickTable3x3 = [
			[ new(0, 0), new(-1, 0), new(-1, -1), new(0,  2), new(-1,  2) ], // 0 -> R
			[ new(0, 0), new( 1, 0), new( 1,  1), new(0, -2), new( 1, -2) ], // R -> 2
			[ new(0, 0), new( 1, 0), new( 1, -1), new(0,  2), new( 1,  2) ], // 2 -> L
			[ new(0, 0), new(-1, 0), new(-1,  1), new(0, -2), new(-1, -2) ], // L -> 0
		];
		public static readonly Vector2I[][] WallKickTable4x4 =
		[
			[ new(0, 0), new(-2, 0), new( 1, 0), new(-2,  1), new( 1, -2) ], // 0 -> R
			[ new(0, 0), new(-1, 0), new( 2, 0), new(-1, -2), new( 2,  1) ], // R -> 2
			[ new(0, 0), new( 2, 0), new(-1, 0), new( 2, -1), new(-1,  2) ], // 2 -> L
			[ new(0, 0), new( 1, 0), new(-2, 0), new( 1,  2), new(-2, -1) ], // L -> 0
		];
		
		public static readonly System.Collections.Generic.Dictionary<SpinType, uint[]> SpinPoints = new()
		{
			{SpinType.None,      [0,   0,   0,    0,    0]},
			{SpinType.TSpin,     [400, 800, 1200, 1600, 2000]},
			{SpinType.TSpinMini, [100, 200, 400,  800,  1000]},
			{SpinType.AllSpin,   [100, 400, 800,  1200, 1800]}
		};

		public static readonly uint[] DestroyedLinesScores = [0, 100, 300, 700, 1500];

		public static Vector2I GetWallKickOffset(Vector2I[][] Table, int OldState, int NewState, int Attempt)
		{
			if (Attempt >= Table[0].Length) return Vector2I.Zero;

			if (OldState == Figure.ROTATION_0 && NewState == Figure.ROTATION_R) return Table[0][Attempt];
			if (OldState == Figure.ROTATION_R && NewState == Figure.ROTATION_0) return -Table[0][Attempt];
			if (OldState == Figure.ROTATION_R && NewState == Figure.ROTATION_2) return Table[1][Attempt];
			if (OldState == Figure.ROTATION_2 && NewState == Figure.ROTATION_R) return -Table[1][Attempt];
			if (OldState == Figure.ROTATION_2 && NewState == Figure.ROTATION_L) return Table[2][Attempt];
			if (OldState == Figure.ROTATION_L && NewState == Figure.ROTATION_2) return -Table[2][Attempt];
			if (OldState == Figure.ROTATION_L && NewState == Figure.ROTATION_0) return Table[3][Attempt];
			if (OldState == Figure.ROTATION_0 && NewState == Figure.ROTATION_L) return -Table[3][Attempt];

			return Vector2I.Zero;
		}
	}

	public class Figure
	{
		private static readonly FigureType[] _figureTypes = (FigureType[])Enum.GetValues(typeof(FigureType));
		private static readonly FigureColor[] _figureColors = (FigureColor[])Enum.GetValues(typeof(FigureColor));

		public const byte ROTATION_0 = 0;
		public const byte ROTATION_R = 1;
		public const byte ROTATION_2 = 2;
		public const byte ROTATION_L = 3;
		private static readonly Vector2I DEFAULT_POSITION = new(3, 0);

		public enum FigureType { O, I, J, L, S, Z, T };

		public enum FigureColor
		{
			White,
			Green,
			Yellow,
			Blue,
			Purple,
			Cyan,
			Red,
		}

		public static readonly Color[] FigureColors =
		[
			Colors.White,
			Colors.Green,
			Colors.Yellow,
			Colors.DodgerBlue,
			Colors.Purple,
			Colors.Aqua,
			Colors.Red,
		];
		private bool[][] _form;
		private Vector2I _size;
		private Rect2I _activeZone;

		public Vector2I Position = DEFAULT_POSITION;
		public byte Rotation = ROTATION_0;
		public FigureType Type = FigureType.O;
		public FigureColor Color = FigureColor.White;
		public Vector2I Size { get => _size; }
		public Rect2I ActiveZone { get => _activeZone; }
		public int Length { get => Form.Length; }
		public bool[][] Form
		{
			get => _form;
			set
			{
				_form = value;
				_size = new(value.Length, value[0].Length);
				_activeZone = GetActiveZone(value);
			}
		}

		public Figure() // Random Figure
		{
			FigureType RandomType = GetRandomFigureType();
			Type = RandomType;
			Form = Forms[(int)RandomType];
			Color = GetRandomFigureColor();
		}
		public Figure(Vector2I Pos) // Random Figure
		{
			FigureType RandomType = GetRandomFigureType();
			Type = RandomType;
			Position = Pos;
			Form = Forms[(int)RandomType];
			Color = GetRandomFigureColor();
		}

		public bool[] this[int id]
		{
			get => Form[id];
		}

		public void Rotate(bool Reverse = false)
		{
			if (Reverse)
			{
				Rotation += 1;
				if (Rotation > 3)
					Rotation = ROTATION_0;
			}
			else
			{
				Rotation -= 1;
				if (Rotation < 0)
					Rotation = ROTATION_L;
			}
			Form = GetRotatedForm(Reverse);
		}

		public bool[][] GetRotatedForm(bool Reverse = false)
		{
			if (Form.Length == 2) return Form;

			List<bool[]> RotatedForm = [];

			// rotating matrix
			if (Reverse)
				for (int X = 0; X < Form.Length; X++)
				{
					List<bool> NewRow = [];
					for (int Y = Form[0].Length - 1; Y >= 0; Y--)
						NewRow.Add(Form[Y][X]);
					RotatedForm.Add([.. NewRow]);
				}
			else
				for (int X = Form[0].Length - 1; X >= 0; X--)
				{
					List<bool> NewRow = [];
					for (int Y = 0; Y < Form.Length; Y++)
						NewRow.Add(Form[Y][X]);
					RotatedForm.Add([.. NewRow]);
				}
			return [.. RotatedForm];
		}

		/// <summary>
		/// Recalculate active zone, recommended to use <c>Figure.ActiveZone</c> instead
		/// </summary>
		/// <returns></returns>
		public Rect2I GetActiveZone()
		{
			Rect2I Result = new(Size.X, Size.Y, -1, -1);
			for (int X = 0; X < Size.X; X++)
				for (int Y = 0; Y < Size.Y; Y++)
					if (_form[X][Y])
					{
						if (Result.Position.X > X) Result.Position = new(X, Result.Position.Y);
						if (Result.Position.Y > Y) Result.Position = new(Result.Position.X, Y);
						if (Result.Size.X < X) Result.Size = new(X, Result.Size.Y);
						if (Result.Size.Y < Y) Result.Size = new(Result.Size.X, Y);
					}
			Result.Size -= Result.Position;
			Result.Size += Vector2I.One;
			return Result;
		}
		/// <summary>
		/// Static variant
		/// </summary>
		/// <param name="Form"></param>
		/// <returns></returns>
		public static Rect2I GetActiveZone(bool[][] Form)
		{
			Vector2I Size = GetFormSize(Form);
			Rect2I Result = new(Size.X, Size.Y, -1, -1);
			for (int X = 0; X < Size.X; X++)
				for (int Y = 0; Y < Size.Y; Y++)
					if (Form[X][Y])
					{
						if (Result.Position.X > X) Result.Position = new(X, Result.Position.Y);
						if (Result.Position.Y > Y) Result.Position = new(Result.Position.X, Y);
						if (Result.Size.X < X) Result.Size = new(X, Result.Size.Y);
						if (Result.Size.Y < Y) Result.Size = new(Result.Size.X, Y);
					}
			Result.Size -= Result.Position;
			Result.Size += Vector2I.One;
			return Result;
		}

		public Color GetColor() => GetColorFromType(Color);

		public static FigureColor GetRandomFigureColor() => _figureColors[GD.Randi() % (_figureColors.Length - 1) + 1];
		public static FigureType GetRandomFigureType() => _figureTypes[GD.Randi() % (_figureTypes.Length - 1) + 1];
		public static Color GetColorFromType(FigureColor BlockType) => FigureColors[(int)BlockType];
		public static Vector2I GetFormSize(bool[][] Form) => new(Form.Length, Form[0].Length);

		// Figures
		public static readonly bool[][][] Forms = [
			[
				[true,  true],
				[true,  true],
			],
			[
				[false, true,   false,  false],
				[false, true,   false,  false],
				[false, true,   false,  false],
				[false, true,   false,  false]
			],
			[
				[false, true,   false],
				[false, true,   false],
				[true,  true,   false]
			],
			[
				[true,  true,   false],
				[false, true,   false],
				[false, true,   false]
			],
			[
				[true,  false,  false],
				[true,  true,   false],
				[false, true,   false]
			],
			[
				[false, true,   false],
				[true,  true,   false],
				[true,  false,  false]
			],
			[
				[false, true,   false],
				[true,  true,   false],
				[false, true,   false]
			],
		];
	}
}
