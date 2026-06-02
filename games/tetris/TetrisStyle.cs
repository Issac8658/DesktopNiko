using Godot;
using System;
using System.Collections.Generic;
using Tetris;

public partial class TetrisStyle : Control
{
	// styles themselves are a bit inappropriate, but I couldn't think of anything better
	private readonly string[] StylesTexts = [
		" ", // no style
		"[center][color=#AB8346]FLICKER[/color][/center]", // D FLICKER
		"[center][color=#CEA261]DIM LIGHT[/color][/center]", // C DIM LIGHT
		"[center][color=#FFF9CC]STEADY GLOW[/color][/center]", // B STEADY GLOW
		"[center][color=#FFFFFF]BRIGHT PATH[/color][/center]", // A BRIGHT PATH
		"[center][color=#AAE2FF]GUIDING LIGHT[/color][/center]", // S GUIDING LIGHT
		"[color=#FFFF66][left]THE SUN[/left]\n[right]MUST RISE[/right][/color]", // SS THE SUN MUST RISE
		"[center][color=#FFCC00]CARRY THE SUN[[/color][/center]", // SSS CARRY THE SUN
		"[center][color=#FFD800][outline_size=4][outline_color=#FFFFFF]ONESHOT[/outline_color][/outline_size][/color][/center]", // ONESHOT
	];
	private readonly Dictionary<string, string[]> StyleHistoryTexts = new() {
		{"LineBreaked", ["Line{0} Break{1} - {2}", "FFFFFE"]}, // id, text, color
		{"Combo", ["Combo multiplier x{0}", "FF0"]},
		{"TSpin", ["T-Spin{0} - {1}", "F20"]},
		{"TSpinMini", ["T-Spin Mini{0} - {1}", "FF0"]},
		{"AllSpin", ["All-Spin{0} - {1}", "2F0"]},
		{"MapCleared", ["Map Cleared", "0F0"]},
		{"Tetris", ["Tetris!", "F0F"]}
	};
	private readonly double[] TimeoutSpeedMultipliers = [0.5, 1, 1.25, 1.5, 2, 3, 4, 6, 8];
	private readonly double[] TimeoutCounters = [
		0.2, // figure block
		0.5, // 1 line
		1.0, // 2 lines
		2.0, // 3 lines
		3.0 // 4 lines
	];
	private readonly Dictionary<TetrisGameController.SpinType, double> SpinStylePoints = new() {
		{TetrisGameController.SpinType.None, 0},
		{TetrisGameController.SpinType.TSpin, 0.8},
		{TetrisGameController.SpinType.TSpinMini, 0.4},
		{TetrisGameController.SpinType.AllSpin, 0.4}
	};

	private int _currentStyle = 0;
	private double _styleTimeOut = 0;

	public int CurrentStyle
	{
		get => _currentStyle;
		set
		{
			value = Mathf.Clamp(value, 0, StylesTexts.Length - 1);
			StyleLabel.Text = StylesTexts[value];
			_currentStyle = value;
			if (value == StylesTexts.Length - 1)
			{
				if (!AchievementsController.IsAchievementTakedStatic("tetris_oneshot"))
					AchievementsController.TakeAchievementStatic("tetris_oneshot");
			}
		}
	}

	[Export]
	public TetrisGameController Controller;
	[Export]
	public RichTextLabel StyleLabel;
	[Export]
	public Control StyleTimeOutFiller;
	[Export]
	public Control StyleList;
	[Export]
	public Label ComboLabel;

	public override void _Ready()
	{
		Controller.BlockDropped += (LinesCount, Spin) =>
		{
			if (Spin != TetrisGameController.SpinType.None)
			{
				string text = Spin.ToString();
				DrawHistory(
					StyleHistoryTexts[text][0],
					StyleHistoryTexts[text][1],
					LinesCount > 0 ? $" + {LinesCount} Line{(LinesCount > 1 ? "s" : "")}" : "",
					TetrisGameController.SpinPoints[Spin][LinesCount].ToString()
				);
			}
			else if (LinesCount > 0 && LinesCount < 4)
				DrawHistory(
					StyleHistoryTexts["LineBreaked"][0],
					StyleHistoryTexts["LineBreaked"][1],
					LinesCount > 1 ? "s" : "",
					LinesCount > 1 ? $" ({LinesCount})" : "",
					TetrisGameController.DestroyedLinesScores[LinesCount].ToString()
				);
			if (LinesCount == 4)
			{
				DrawHistory(
					StyleHistoryTexts["Tetris"][0],
					StyleHistoryTexts["Tetris"][1]
				);
			}
			
			ComboLabel.Visible = Controller.Combo > 1;
			ComboLabel.Text = string.Format(StyleHistoryTexts["Combo"][0], Controller.Combo);
			
			_styleTimeOut += TimeoutCounters[LinesCount] * Mathf.Clamp(Controller.Combo, 1.0, double.MaxValue) + SpinStylePoints[Spin];
			if (_styleTimeOut >= 1)
			{
				CurrentStyle += (int)Mathf.Floor(_styleTimeOut);
				_styleTimeOut -= (int)Mathf.Floor(_styleTimeOut);
			}
		};
		Controller.Restarted += () =>
		{
			_styleTimeOut = CurrentStyle = 0;
			foreach (Node child in StyleList.GetChildren())
				child.QueueFree();
		};
		CurrentStyle = 0;
		StyleTimeOutFiller.AnchorRight = (float)_styleTimeOut;
	}

	public override void _Process(double delta)
	{
		if (_styleTimeOut > 0 || CurrentStyle > 0)
		{
			_styleTimeOut -= Controller.CurrentState == TetrisGameController.GameStates.Playing ? delta / 10 * TimeoutSpeedMultipliers[CurrentStyle] : 0;
			if (_styleTimeOut <= 0)
			{
				CurrentStyle -= 1;
				_styleTimeOut = 1;
			}
			StyleTimeOutFiller.AnchorRight = (float)_styleTimeOut;
		}
	}

	private void DrawHistory(string text, string color, params string[] insertStrings)
	{
		string ResultText = string.Format(text, insertStrings);
		if (StyleList.GetChildCount() > 0)
		{
			Label FirstLabel = StyleList.GetChild<Label>(0);
			if (FirstLabel.Text.StartsWith(ResultText))
			{
				string[] splited = FirstLabel.Text.Replace(ResultText, null).Split("x");
				FirstLabel.Text = $"{ResultText} x{(splited[^1] == "" ? 1 : int.Parse(splited[^1])) + 1}";
				FirstLabel.Modulate = new(FirstLabel.Modulate.R, FirstLabel.Modulate.G * 0.875f, FirstLabel.Modulate.B * 0.75f);
				return;
			}
		}


        Label Label = new()
        {
            Text = ResultText,
            Modulate = Color.FromString(color, Colors.Gray),
            UseParentMaterial = true,
            AutowrapMode = TextServer.AutowrapMode.WordSmart
        };
		Label.AddThemeFontSizeOverride("font_size", 16);
        StyleList.AddChild(Label);
		StyleList.MoveChild(Label, 0);
    }
}
