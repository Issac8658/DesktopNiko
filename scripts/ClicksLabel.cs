using Godot;
using System;

public partial class ClicksLabel : Label
{
	private ValuesContainer _valuesContainer;
	public override void _Ready()
	{
		_valuesContainer = GetNode("/root/ValuesContainer") as ValuesContainer;
		Text = _valuesContainer.Clicks.ToString();
		_valuesContainer.Clicked += diff =>
		{
			Text = _valuesContainer.Clicks.ToString();
		};
	}
}
