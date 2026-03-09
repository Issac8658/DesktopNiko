using Godot;
using System;

public partial class CpsCounter : Label
{
	private ValuesContainer _valuesContainer;
	
	public override void _Ready()
	{
		_valuesContainer = GetNode<ValuesContainer>("/root/ValuesContainer");

		_valuesContainer.GlobalTimerTicked += () =>
		{
			Text = _valuesContainer.CPS.ToString();
		};
	}
}
