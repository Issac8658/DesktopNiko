using Godot;
using System;
using System.Collections.Generic;

public partial class TetrisStyle : Control
{
	private readonly string[] StylesTexts = [ // styles themselves are a bit inappropriate, but I couldn't think of anything better
		" ", // no style
		"[color=dodger_blue]FLICKER[/color]", // D
		"[color=dodger_blue]DIM LIGHT[/color]", // C
		"[color=dodger_blue][left]STEADY\n[right]GLOW[/color]", // B
		"[color=dodger_blue][left]BRIGHT\n[right]PATH[/color]", // A
		"[color=dodger_blue][left]GUIDING\n[right]LIGHT[/color]", // S
		"[color=dodger_blue][left]THE SUN\n[right]MUST RISE[/color]", // SS
		"[color=dodger_blue][left]CARRY\n[right]THE SUN[/color]", // SSS
		"[color=dodger_blue]ONESHOT[/color]", // ONESHOT
	];
	private readonly double[] TimeoutSpeedMultipliers = [0.5, 1, 1.25, 1.5, 2, 3, 4, 6, 8];
	private readonly double[] TimeoutCounters = [
		0.2, // figure block
		0.5, // 1 line
		1.0, // 2 lines
		2.0, // 3 lines
		3.0 // 4 lines
	];

	private int _currentStyle = -1;
	private double _styleTimeOut = 0.05;

	public int CurrentStyle
	{
		get => _currentStyle;
		set
		{
			StyleLabel.Text = StylesTexts[value];
			_currentStyle = value;
		}
	}

	[Export]
	public TetrisGameController Controller;
	[Export]
	public RichTextLabel StyleLabel;
	[Export]
	public Control StyleTimeOutFiller;
	[Export]
	public PackedScene StylePonintLabelTemplate;

	public override void _Ready()
	{
		Controller.BlockDropped += LinesCount =>
		{
			_styleTimeOut += TimeoutCounters[LinesCount] * Mathf.Clamp(Controller.Combo, 1.0, double.MaxValue);
			if (_styleTimeOut >= 1)
			{
				CurrentStyle += (int)Mathf.Floor(_styleTimeOut);
				_styleTimeOut -= (int)Mathf.Floor(_styleTimeOut);
			}
		};
		CurrentStyle = 0;
	}

	public override void _Process(double delta)
	{
		if (_styleTimeOut > 0 || CurrentStyle > 0)
		{
			_styleTimeOut -= delta / 10 * TimeoutSpeedMultipliers[CurrentStyle];
			if (_styleTimeOut <= 0)
			{
				CurrentStyle -= 1;
				_styleTimeOut = 1;
			}
			StyleTimeOutFiller.AnchorRight = (float)_styleTimeOut;
		}
	}
}
