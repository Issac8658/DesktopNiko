using Godot;
using System;
using System.Collections.Generic;

public partial class TetrisGameController : Window
{

	public const int GAME_HEIGHT = 20;
	public const int GAME_WIDTH = 10;
	public const double START_TICK_TIME = 0.5;

    public const int STATE_0 = 0;
    public const int STATE_R = 1;
    public const int STATE_2 = 2;
    public const int STATE_L = 3;

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
	public delegate void BlockDroppedEventHandler(int DestroyedLinesCount);
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

	public enum BlockType
	{
		White,
		Green,
		Yellow,
		Blue,
		Purple,
		Cyan,
		Red,
	}
	public static readonly Color[] BlockColors =
	[
		Colors.White,
		Colors.Green,
		Colors.Yellow,
		Colors.DodgerBlue,
		Colors.Purple,
		Colors.Aqua,
		Colors.Red,
	];

	private static readonly Vector2I FIGURE_DEFAULT_POSITION = new(3, 0);
	private static readonly BlockType[] _blockTypes = (BlockType[])Enum.GetValues(typeof(BlockType));

	public static readonly AudioEffectLowPassFilter LowPassFilter = AudioServer.GetBusEffect(2, 0) as AudioEffectLowPassFilter;

	private List<List<BlockType?>> _blocks = [];
	private List<bool[][]> _nextFigures = [];
	private bool[][] _currentFigure = [];
	private Vector2I _currentFigurePosition = FIGURE_DEFAULT_POSITION; // this is position of figure center
	private BlockType _currentFigureType;

	private uint _score = 0;
	private uint _lines = 0;
	private uint _combo = 0;

	private GameStates _currentState = GameStates.Menu;
	
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

	public bool[][] CurrentFigure
	{
		get => _currentFigure;
	}
	public Vector2I CurrentFigurePosition
	{
		get => _currentFigurePosition;
	}
	public bool[][][] NextFigures
	{
		get => [.. _nextFigures];
	}
	public BlockType CurrentFigureType
	{
		get => _currentFigureType;
	}

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

	public List<List<BlockType?>> Blocks { get => _blocks; }

	public override void _Ready()
	{
		for (int Y = 0; Y < GAME_HEIGHT; Y++) // init map
		{
			List<BlockType?> Line = [];
			for (int X = 0; X < GAME_HEIGHT; X++)
				Line.Add(null);
			_blocks.Add(Line);
		}

		for (int i = 0; i < 4; i++) // filling next figures
			_nextFigures.Add(GetRandomFigure());

		_currentFigure = GetRandomFigure(); // getting current figure
		_currentFigureType = GetRandomBlockType();

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

		// init redraw
		ReDraw();
		UpdateRulers();

		int CurrentScreen = DisplayServer.WindowGetCurrentScreen(GetWindowId());
		
		Size = DisplayServer.ScreenGetSize(CurrentScreen) - new Vector2I(20,0);
		Position = DisplayServer.ScreenGetPosition(CurrentScreen) + new Vector2I(10,0);
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
			else if (@event.IsActionPressed("down")) // s, ↓
			{
				TickCurrentFigure(); // move current figure down for 1 block
				ReDrawCurrentFigure();
			}
			else if (@event.IsActionPressed("dialog_skip")) // space, enter, z
			{
				while (!TickCurrentFigure(true)); // move current figure to bottom
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
		ReDrawCurrentFigure(); // figure redraw
		UpdateStats(); // stats update
	}

	private void ReDrawCurrentFigure(bool Drop = false) // redrawing current figure
	{
		if (Drop)
			EmitSignal("ReDrawZone", new Rect2I(_currentFigurePosition.X - 2, 0, 4 + GetFigureMatrixSize(CurrentFigure).X, GAME_HEIGHT)); // full colums if pressing space
		else
			EmitSignal("ReDrawZone", new Rect2I(_currentFigurePosition - new Vector2I(2,2), new Vector2I(4,4) + GetFigureMatrixSize(CurrentFigure))); // else zone around figure
		
		UpdateRulers();
	}

	private void UpdateRulers()
	{
		Rect2I ActiveZone = GetFigureActiveZone(_currentFigure);
		Vector2I ActiveZonePosition = _currentFigurePosition + ActiveZone.Position;

		LeftRuler.Position = new(ActiveZonePosition.X * 40 - 3, 0); // positing left ruler
		RightRuler.Position = LeftRuler.Position + new Vector2(ActiveZone.Size.X * 40, 0); // positing right ruler
		RightRuler.Modulate = LeftRuler.Modulate = GetColorFromType(CurrentFigureType);
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

	private bool TickCurrentFigure(bool HeightPlace = false)
	{
		if (FigureCanMove(_currentFigure, _currentFigurePosition + new Vector2I(0, 1))) // checking if current figure can move
		{
			_currentFigurePosition.Y += 1; // moving
			_score += 1;
			return false;
		}
		else
		{
			if (_currentFigurePosition.Y < 3) // lose checking
				CurrentState = GameStates.Lose;
			else
			{
				// redrawing and figure place and some similar shit idk

				ReDrawCurrentFigure(true);
				PlaceCurrentFigure();
				_currentFigure = _nextFigures[0];
				_nextFigures.RemoveAt(0);
				_nextFigures.Add(GetRandomFigure());
				_currentFigurePosition = FIGURE_DEFAULT_POSITION;
				_score += 10;
				EmitSignal("ReDrawNextTiles");
				ReDrawCurrentFigure(true);
			}
		}

		// LinesChecking
		
		uint DestroyedLines = 0;

		for (int Y = 0; Y < GAME_HEIGHT; Y++)
		{
			bool lineFull = false;
			for (int X = 0; X < GAME_WIDTH; X++)
			{
				if (_blocks[X][Y] != null)
					lineFull = true;
				else
				{
					lineFull = false;
					break;
				}
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
		switch (DestroyedLines)
		{
			case 0:
				if (HeightPlace)
					HeightPlaceSound.Play();
				else
					PlaceSound.Play();
				break;
			case 1:
				PlaceLightSound.Play();
				_score += 100 * Combo;
				break;
			case 2:
				PlaceNormalSound.Play();
				_score += 300 * Combo;
				break;
			case 3:
				PlaceMegaSound.Play();
				_score += 700 * Combo;
				break;
			case 4:
				PlaceMegaSound.Play();
				_score += 1500 * Combo;
				break;
		}
		EmitSignal("BlockDropped", DestroyedLines);


		_lines += DestroyedLines;

		return true;
	}

	private void RestartGame() // full restart
	{
		_blocks = [];
		for (int Y = 0; Y < GAME_HEIGHT; Y++)
		{
			List<BlockType?> Line = [];
			for (int X = 0; X < GAME_HEIGHT; X++)
				Line.Add(null);
			_blocks.Add(Line);
		}
		_nextFigures = [];
		for (int i = 0; i < 4; i++)
			_nextFigures.Add(GetRandomFigure());

		_currentFigure = GetRandomFigure();
		_currentFigurePosition = FIGURE_DEFAULT_POSITION;
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
		if (FigureCanMove(_currentFigure, _currentFigurePosition + Offset))
			_currentFigurePosition += Offset;
		MoveCurrentFigureInBorders();
	}

	private bool FigureCanMove(bool[][] Figure, Vector2I Position)
	{
		if (Position == _currentFigurePosition)
			return true;

		Rect2I ActiveZone = GetFigureActiveZone(Figure);
		Vector2I ActiveZonePosition = Position + ActiveZone.Position;
		
		if (ActiveZonePosition.X < 0
		 || ActiveZonePosition.X + ActiveZone.Size.X - 1 >= GAME_WIDTH
		 || ActiveZonePosition.Y + ActiveZone.Size.Y - 1 >= GAME_HEIGHT)
			return false;
		
		for (int X = ActiveZone.Position.X; X < ActiveZone.Position.X + ActiveZone.Size.X; X++)
			for (int Y = ActiveZone.Position.Y; Y < ActiveZone.Position.Y + ActiveZone.Size.Y; Y++)
				if (Figure[X][Y])
				{
					Vector2I globalPos = new Vector2I(X, Y) + Position;
					if (_blocks[globalPos.X][globalPos.Y] != null)
						return false;
				}
		return true;
	}

	private void MoveCurrentFigureInBorders() // if figure want to go out of bounds
	{
		Rect2I ActiveZone = GetFigureActiveZone(_currentFigure);
		int ActiveZonePosition = _currentFigurePosition.X + ActiveZone.Position.X;
		_currentFigurePosition.X = Mathf.Clamp(ActiveZonePosition, 0, GAME_WIDTH - ActiveZone.Size.X + 1) - ActiveZone.Position.X;
	}
	
	private int GetFigureInBordersOffset() =>
		Math.Clamp(
			_currentFigurePosition.X,
			0,
			GAME_WIDTH - GetFigureMatrixSize(CurrentFigure).X
		) - _currentFigurePosition.X;

	private void RotateCurrentFigure() // figure rotating
	{
		GD.Print("Rotating");
		List<bool[]> RotatedFigure = [];

		for (int X = _currentFigure[0].Length - 1; X >= 0; X--)
		{
			List<bool> NewRow = [];
			for (int Y = 0; Y < _currentFigure.Length; Y++)
				NewRow.Add(_currentFigure[Y][X]);
			RotatedFigure.Add([.. NewRow]);
		}
		bool[][] RotatedFigureArray = [.. RotatedFigure];
		if (FigureCanMove(RotatedFigureArray, _currentFigurePosition))
		{
			GD.Print("Rotaded");
			_currentFigure = RotatedFigureArray;
			//MoveCurrentFigureInBorders();
		}
	}

	private void PlaceCurrentFigure()
	{
		for (int X = 0; X < GAME_WIDTH; X++)
			for (int Y = 0; Y < GAME_HEIGHT; Y++)
				if (CurrentFigureIsHere(new(X, Y)))
					_blocks[X][Y] = _currentFigureType;
		_currentFigureType = GetRandomBlockType();
	}

	public bool CurrentFigureIsHere(Vector2I Position) // checking if current figure's block in this pos 
	{
		Vector2I FigureSize = GetFigureMatrixSize(_currentFigure);

		if (Position.X - _currentFigurePosition.X >= 0 && Position.X - _currentFigurePosition.X < FigureSize.X &&
			Position.Y - _currentFigurePosition.Y >= 0 && Position.Y - _currentFigurePosition.Y < FigureSize.Y) // wtf is this that..?
			return _currentFigure[Position.X - _currentFigurePosition.X][Position.Y - _currentFigurePosition.Y];
			
		return false;
	}

	private void TweenLowPass(float result)
	{
		Tween tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Quint);
		tween.TweenProperty(LowPassFilter, "cutoff_hz", result, 0.25);
	}

	public Vector2I GetCurrentFigureCenterPosition() => _currentFigurePosition + GetFigureCenterOffset(_currentFigure); // Getting top left corner of current figure

	public static Rect2I GetFigureActiveZone(bool[][] Figure)
	{
		Rect2I Result = new(Figure.Length, Figure[0].Length, -1, -1);
		for (int X = 0; X < Figure.Length; X++)
			for (int Y = 0; Y < Figure[0].Length; Y++)
				if (Figure[X][Y])
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

	public static bool[][] GetRandomFigure() => _figures[GD.Randi() % _figures.Length]; // getting random figure

	public static Vector2I GetFigureMatrixSize(bool[][] Figure) => new(Figure.Length, Figure[0].Length); // figure size

	public static Vector2I GetFigureCenterOffset(bool[][] Figure) => GetFigureMatrixSize(Figure) / 2; // get center of figure

	public static BlockType GetRandomBlockType() => _blockTypes[GD.Randi() % (_blockTypes.Length - 1) + 1];

	public static Color GetColorFromType(BlockType BlockType) => BlockColors[(int)BlockType];

	// Figures
	public static readonly bool[][][] _figures = [ // figures
		[
			[true,	true],
			[true,	true],
		],
		[
			[false,	true,	false,	false],
			[false,	true,	false,	false],
			[false,	true,	false,	false],
			[false,	true,	false,	false]
		],
		[
			[false,	true, 	false],
			[false,	true, 	false],
			[true,	true, 	false]
		],
		[
			[true,	true, 	false],
			[false,	true, 	false],
			[false,	true, 	false]
		],
		[
			[true,	false, 	false],
			[true,	true, 	false],
			[false,	true, 	false]
		],
		[
			[false,	true, 	false],
			[true,	true, 	false],
			[true,	false, 	false]
		],
		[
			[false,	true, 	false],
			[true,	true, 	false],
			[false,	true, 	false]
		],
	];

	public static readonly Vector2I[][] WallKickTables3x3 = [
        [ new(0, 0), new(-1, 0), new(-1, -1), new(0,  2), new(-1,  2) ], // 0 -> R
        [ new(0, 0), new( 1, 0), new( 1,  1), new(0, -2), new( 1, -2) ], // R -> 0
        [ new(0, 0), new( 1, 0), new( 1,  1), new(0, -2), new( 1, -2) ], // R -> 2
        [ new(0, 0), new(-1, 0), new(-1, -1), new(0,  2), new(-1,  2) ], // 2 -> R
        [ new(0, 0), new( 1, 0), new( 1, -1), new(0,  2), new( 1,  2) ], // 2 -> L
        [ new(0, 0), new(-1, 0), new(-1,  1), new(0, -2), new(-1, -2) ], // L -> 2
        [ new(0, 0), new(-1, 0), new(-1,  1), new(0, -2), new(-1, -2) ], // L -> 0
        [ new(0, 0), new( 1, 0), new( 1, -1), new(0,  2), new( 1,  2) ]  // 0 -> L
	];
	public static readonly Vector2I[][] WallKickTables4x4 =
    [
        [ new(0, 0), new(-2, 0), new( 1, 0), new(-2,  1), new( 1, -2) ], // 0 -> R
        [ new(0, 0), new( 2, 0), new(-1, 0), new( 2, -1), new(-1,  2) ], // R -> 0
        [ new(0, 0), new(-1, 0), new( 2, 0), new(-1, -2), new( 2,  1) ], // R -> 2
        [ new(0, 0), new( 1, 0), new(-2, 0), new( 1,  2), new(-2, -1) ], // 2 -> R
        [ new(0, 0), new( 2, 0), new(-1, 0), new( 2, -1), new(-1,  2) ], // 2 -> L
        [ new(0, 0), new(-2, 0), new( 1, 0), new(-2,  1), new( 1, -2) ], // L -> 2
        [ new(0, 0), new( 1, 0), new(-2, 0), new( 1,  2), new(-2, -1) ], // L -> 0
        [ new(0, 0), new(-1, 0), new( 2, 0), new(-1, -2), new( 2,  1) ]  // 0 -> L
    ];
	
}
