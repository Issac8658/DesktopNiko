using Godot;
using System;
using System.Collections.Generic;

public partial class TetrisGameController : Window
{

	public const int GAME_HEIGHT = 20;
	public const int GAME_WIDTH = 10;

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
	public delegate void BlockDroppedEventHandler(bool LineDestroyed);
	[Signal]
	public delegate void ComboChangedEventHandler(uint Combo);

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
	private static readonly Vector2I FIGURE_DEFAULT_POSITION = new(5, 1);

	private List<List<BlockType?>> _blocks = [];
	private List<bool[][]> _nextFigures = [];
	private bool[][] _currentFigure = [];
	private Vector2I _currentFigurePosition = FIGURE_DEFAULT_POSITION;

	private uint _score = 0;
	private uint _lines = 0;
	private uint _combo = 0;

	private GameStates _currentState = GameStates.Menu;
	
	private uint Combo
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
	public Vector2I CurrentFigurePositiontFigure
	{
		get => _currentFigurePosition;
	}
	public bool[][][] NextFigures
	{
		get => [.. _nextFigures];
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

	public List<List<BlockType?>> Blocks
	{
		get => _blocks;
	}

	public override void _Ready()
	{
		for (int Y = 0; Y < GAME_HEIGHT; Y++) // init map
		{
			List<BlockType?> Line = [];
			for (int X = 0; X < GAME_HEIGHT; X++)
			{
				Line.Add(null);
			}
			_blocks.Add(Line);
		}

		for (int i = 0; i < 4; i++) // filling next figures
			_nextFigures.Add(GetRandomFigure());

		_currentFigure = GetRandomFigure(); // getting current figure


		GameTimer.Timeout += Tick; // ticks timer

		StateChanged += () => // states
		{
			switch (CurrentState)
			{
				case GameStates.Restart:
					RestartGame();
					CurrentState = GameStates.Playing;
					break;
				case GameStates.Playing:
					GameTimer.Start();
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
			if (CurrentState == GameStates.Playing) CurrentState = GameStates.Paused;
			else if (CurrentState == GameStates.Paused) CurrentState = GameStates.Playing;
		};
		ExitButton.Pressed += () => CurrentState = GameStates.Menu; // menu

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
		Vector2I pos = _currentFigurePosition - GetFigureCenterOffset(_currentFigure);
		if (Drop)
			EmitSignal("ReDrawZone", new Rect2I(pos.X - 2, 0, 4 + GetFigureSize(CurrentFigure).X, GAME_HEIGHT)); // full colums if pressing space
		else
			EmitSignal("ReDrawZone", new Rect2I(pos - new Vector2I(2,2), new Vector2I(4,4) + GetFigureSize(CurrentFigure))); // else zone around figure
		

		UpdateRulers();
	}

	private void UpdateRulers()
	{
		LeftRuler.Position = new((_currentFigurePosition.X - GetFigureCenterOffset(_currentFigure).X) * 40 - 3, 0); // positing left ruler
		RightRuler.Position = LeftRuler.Position + new Vector2(_currentFigure.Length * 40, 0); // positing right ruler
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
		Vector2I pos = _currentFigurePosition - GetFigureCenterOffset(_currentFigure); // getting top left corner of current figure
		if (FigureCanMove(_currentFigure, pos + new Vector2I(0, 1))) // checking if current figure can move
		{
			_currentFigurePosition.Y += 1; // moving
			_score += 1;
			return false;
		}
		else
		{
			if (pos.Y < 3) // lose checking
			{
				CurrentState = GameStates.Lose;
			}
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

		uint DestroyedLines = 0;

		// LinesChecking
		for (int Y = 0; Y < GAME_HEIGHT; Y++)
		{
			bool lineFull = false;
			for (int X = 0; X < GAME_WIDTH; X++)
			{
				if (_blocks[X][Y] != null)
				{
					lineFull = true;
				}
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
		EmitSignal("BlockDropped", DestroyedLines > 0);


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
			{
				Line.Add(null);
			}
			_blocks.Add(Line);
		}
		_nextFigures = [];
		for (int i = 0; i < 4; i++)
			_nextFigures.Add(GetRandomFigure());

		_currentFigure = GetRandomFigure();
		_currentFigurePosition = FIGURE_DEFAULT_POSITION;
		_score = 0;
		_lines = 0;
		ReDraw();
		EmitSignal("ReDrawNextTiles");
	}

	private void MoveCurrentFigure(Vector2I Offset)
	{
		if (FigureCanMove(_currentFigure, _currentFigurePosition - GetFigureCenterOffset(_currentFigure) + Offset))
		{
			_currentFigurePosition += Offset;
		}
		MoveCurrentFigureInBorders();
	}

	private bool FigureCanMove(bool[][] Figure, Vector2I Position)
	{
		Vector2I size = GetFigureSize(Figure);
		bool canMove = false;
		for (int X = 0; X < size.X; X++)
		{
			for (int Y = 0; Y < size.Y; Y++)
			{
				if (Figure[X][Y])
				{
					Vector2I globalPos = new Vector2I(X, Y) + Position;
					if (globalPos.X >= 0 && globalPos.Y >= 0)
					{
						canMove = globalPos.Y < GAME_HEIGHT && _blocks[globalPos.X][globalPos.Y] == null;
						if (!canMove)
							break;
					}
				}
			}
			if (!canMove)
				break;
		}
		return canMove;
	}

	private void MoveCurrentFigureInBorders() // if figure want to go out of bounds
	{
		Vector2I pos = GetCurrentFigureRealPosition();
		_currentFigurePosition.X -= Math.Clamp(pos.X, -9999, 0);
		_currentFigurePosition.X += Math.Clamp(GAME_WIDTH - pos.X - GetFigureSize(_currentFigure).X, -9999, 0);
	}

	private void RotateCurrentFigure() // figure rotating
	{
		List<bool[]> RotatedFigure = [];

		for (int X = _currentFigure[0].Length - 1; X >= 0; X--)
		{
			List<bool> NewRow = [];
			for (int Y = 0; Y < _currentFigure.Length; Y++)
			{
				NewRow.Add(_currentFigure[Y][X]);
			}
			RotatedFigure.Add([.. NewRow]);
		}
		bool[][] RotatedFigureArray = [.. RotatedFigure];
		if (FigureCanMove(RotatedFigureArray, _currentFigurePosition - GetFigureCenterOffset(RotatedFigureArray)))
		{
			_currentFigure = RotatedFigureArray;
			MoveCurrentFigureInBorders();
		}
	}

	private void PlaceCurrentFigure()
	{
		for (int X = 0; X < GAME_WIDTH; X++)
		{
			for (int Y = 0; Y < GAME_HEIGHT; Y++)
			{
				if (CurrentFigureIsHere(new(X, Y)))
				{
					_blocks[X][Y] = BlockType.White;
				}
			}
		}
	}

	public bool CurrentFigureIsHere(Vector2I Position) // checking if current figure's block in this pos 
	{
		Vector2I FigureSize = GetFigureSize(_currentFigure);

		Vector2I CurrentFigureRealPosition = GetCurrentFigureRealPosition();


		if (Position.X - CurrentFigureRealPosition.X >= 0 && Position.X - CurrentFigureRealPosition.X < FigureSize.X &&
			Position.Y - CurrentFigureRealPosition.Y >= 0 && Position.Y - CurrentFigureRealPosition.Y < FigureSize.Y) // wtf is this that..?
			return _currentFigure[Position.X - CurrentFigureRealPosition.X][Position.Y - CurrentFigureRealPosition.Y];
			
		return false;
	}


	public Vector2I GetCurrentFigureRealPosition() // Getting top left corner of current figure
	{
		return _currentFigurePosition - GetFigureCenterOffset(_currentFigure);
	}

	public static bool[][] GetRandomFigure() // getting random figure
	{
		return _figures[GD.Randi() % _figures.Length];
	}

	public static Vector2I GetFigureSize(bool[][] Figure) // figure size
	{
		return new Vector2I(Figure.Length, Figure[0].Length);
	}

	public static Vector2I GetFigureCenterOffset(bool[][] Figure) // get center of figure
	{
		return GetFigureSize(Figure) / 2;
	}

	// Figures
	public static readonly bool[][][] _figures = [ // figures
		[
			[true,	true],
			[true,	true],
		],
		[
			[true,	true,	true,	true]
		],
		[
			[true,	true, 	true],
			[true,	false, 	false]
		],
		[
			[true,	true, 	true],
			[false,	false, 	true]
		],
		[
			[true,	true, 	false],
			[false,	true, 	true]
		],
		[
			[false,	true, 	true],
			[true,	true, 	false]
		],
		[
			[true,	true, 	true],
			[false,	true, 	false]
		],
	];
}
