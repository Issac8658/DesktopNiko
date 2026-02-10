using Godot;
using System;
using System.Collections.Generic;

public partial class TetrisStyle : Control
{
	// styles themselves are a bit inappropriate, but I couldn't think of anything better
	private readonly string[] StylesTexts = [
		" ", // no style
		"[center][color=#AB8346]FLICKER[/color][/center]", // D FLICKER
		"[center][color=#CEA261]DIM LIGHT[/color][/center]", // C DIM LIGHT
		"[color=#FFF9CC][left]STEADY[/left]\n[right]GLOW[/right][/color]", // B STEADY GLOW
		"[color=#FFFFFF][left]BRIGHT[/left]\n[right]PATH[/right][/color]", // A BRIGHT PATH
		"[color=#AAE2FF][left]GUIDING[/left]\n[right]LIGHT[/right][/color]", // S GUIDING LIGHT
		"[color=#FFFF66][left]THE SUN[/left]\n[right]MUST RISE[/right][/color]", // SS THE SUN MUST RISE
		"[color=#FFCC00][left]CARRY[/left]\n[right]THE SUN[/right][/color]", // SSS CARRY THE SUN
		"[center][color=#FFD800][outline_size=4][outline_color=#FFFFFF]ONESHOT[/outline_color][/outline_size][/color][/center]", // ONESHOT
	];
	private readonly double[] TimeoutSpeedMultipliers = [0.5, 1, 1.25, 1.5, 2, 3, 4, 6, 8];
	private readonly double[] TimeoutCounters = [
		0.2, // figure block
		0.5, // 1 line
		1.0, // 2 lines
		2.0, // 3 lines
		3.0 // 4 lines
	];

	private int _currentStyle = 0;
	private double _styleTimeOut = 0;

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
		StyleTimeOutFiller.AnchorRight = (float)_styleTimeOut;
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
