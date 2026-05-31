using Godot;
using Tetris;

public partial class TetrisShake : Node
{
	[Export]
	public TetrisGameController GameController;
	[Export]
	public Control ControlToShake;

	private Vector2 _origPos;
	private double _shakePower = 0f;
	private double _shakeTime = 0f;
	
	public override void _Ready()
	{
		_origPos = ControlToShake.Position;

		GameController.LineDestroyed += Y =>
		{
			_shakePower += 4.0;
		};
	}

	public override void _Process(double delta)
	{
		Shake();
		_shakePower *= 0.95;
		_shakePower = Mathf.Max(_shakePower, 0);
	}

	private void Shake()
	{
		ControlToShake.Position = _origPos + new Vector2((float)GD.RandRange(-_shakePower, _shakePower), (float)GD.RandRange(-_shakePower, _shakePower));
	}
}
