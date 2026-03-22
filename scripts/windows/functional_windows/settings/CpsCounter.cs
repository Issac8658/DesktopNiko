using Godot;
using System;

public partial class CpsCounter : Label
{
	private ValuesContainer _valuesContainer;

	private Timer _timer = new();
	
	public override void _Ready()
	{
		_valuesContainer = GetNode<ValuesContainer>("/root/ValuesContainer");
	}

	public override void _Process(double delta)
	{
		if (IsVisibleInTree())
		{
			Text = _valuesContainer.CPS.ToString();
		}
	}
}
