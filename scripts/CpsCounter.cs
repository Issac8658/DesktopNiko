using Godot;
using System;

public partial class CpsCounter : Label
{
	private ValuesContainer _valuesContainer;

	private int _lastClicks = 0;
	
	public override void _Ready()
	{
		_valuesContainer = GetNode<ValuesContainer>("/root/ValuesContainer");
		_valuesContainer.Clicked += Diff =>
		{
			_lastClicks += Diff;
		};

		_valuesContainer.GlobalTimerTicked += () =>
		{
			Text = _lastClicks.ToString();
			_lastClicks = 0;
		};
	}
}
