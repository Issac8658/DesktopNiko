using Godot;
using System;

public partial class TetrisCombo : Node
{
	[Export]
	public TetrisGameController GameController;
	[Export]
	public Label ComboLabel;
	[Export]
	public Control ComboControl;
	
	private float target_visible_mult = 0;

	public override void _Ready()
	{
		GameController.ComboChanged += Combo =>
		{
			ComboLabel.Text = Combo.ToString();
			if (Combo == 0)
				target_visible_mult = 0;
			else
				target_visible_mult = 1;
		};
	}
	
	public override void _Process(double delta)
	{
		ComboControl.Modulate = new (1, 1, 1, Mathf.Lerp(ComboControl.Modulate.A, target_visible_mult, 0.1f));
	}
}
