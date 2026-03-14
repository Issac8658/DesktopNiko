using Godot;
using System;

public partial class ThemeChangerShortcut : Button
{
	[Export]
	public ColorPalette ThemePallete;
	[Export]
	public bool Rainbow = false;

	private ValuesContainer _valuesContainer;
	public override void _Ready()
	{
		_valuesContainer = GetNode<ValuesContainer>("/root/ValuesContainer");
		Pressed += () =>
		{
			_valuesContainer.ThemeColorMain = ThemePallete.Colors[0];
			_valuesContainer.ThemeColorHover = ThemePallete.Colors[1];
			_valuesContainer.ThemeColorOutlineHover = ThemePallete.Colors[2];
			_valuesContainer.ThemeColorBaseHover = ThemePallete.Colors[3];
			_valuesContainer.ThemeColorOutlinePressed = ThemePallete.Colors[4];
			_valuesContainer.ThemeColorBasePressed = ThemePallete.Colors[5];
			_valuesContainer.ThemeColorBackground = ThemePallete.Colors[6];
			_valuesContainer.ThemeRainbow = Rainbow;
		};
	}
}
