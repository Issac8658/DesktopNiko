using Godot;
using System;
using System.Linq;

public partial class OmnfMenuButtonsController : Control
{
	private int _currentButton = 0;
	[ExportGroup("Color", "Color")]
	[Export]
	public Color ColorSelectedBase;
	[Export]
	public Color ColorSelectedOutline;
	[Export]
	public Color ColorSelectedShadow;
	[Export]
	public Color ColorOtherBase;
	[Export]
	public Color ColorOtherOutline;
	[Export]
	public Color ColorOtherShadow;

	[Export]
	public float SinScaleDevide = 524f;
	[Export]
	public AudioStreamPlayer SwitchSound;
	[Export]
	public AudioStreamPlayer EnterSound;
	[Export]
	public int CurrentButton
	{
		get => _currentButton;
		set
		{
			int ClampedValue = Mathf.Clamp(value, 0, MenuButtons.Length - 1);
			if (_currentButton != ClampedValue) SwitchSound.Play();
			_currentButton = ClampedValue;
		}
	}
	[Export]
	public Control[] MenuButtons;
	public override void _Process(double delta)
	{
		if (IsVisibleInTree())
		{
			for (int i = 0; i < MenuButtons.Length; i++)
			{
				Position = Position.Lerp(new Vector2(0, -128.0f * CurrentButton), 0.15f);

				float sin = Mathf.Sin(MenuButtons[i].GlobalPosition.Y / SinScaleDevide * Mathf.Pi);
				float color_blend = (Mathf.Pow(Mathf.Clamp(-sin, 0f, 1f), 5f) + 1.5f) / 2.5f;
				MenuButtons[i].Position = new Vector2(320f - sin * 100f, 0f);

				foreach (Label Child in MenuButtons[i].GetChildren().Cast<Label>())
				{
					if (Child is Label)
					{
						Child.Set("theme_override_colors/font_color", MixColor(ColorOtherBase, ColorSelectedBase, color_blend));
						Child.Set("theme_override_colors/font_shadow_color", MixColor(ColorOtherShadow, ColorSelectedShadow, color_blend));
						Child.Set("theme_override_colors/font_outline_color", MixColor(ColorOtherOutline, ColorSelectedOutline, color_blend));
					}
					
				}
			}
		}
	}
	public override void _Input(InputEvent Event)
	{
		if (IsVisibleInTree())
		{
			if (Event.IsActionPressed("down"))
			{
				CurrentButton += 1;
			}
			if (Event.IsActionPressed("up"))
			{
				CurrentButton -= 1;
			}
		}
	}

	public static Color MixColor(Color a, Color b, float Blend)
	{
		return Color.Color8(
			Lerp(a.R*255, b.R*255, Blend),
			Lerp(a.G*255, b.G*255, Blend),
			Lerp(a.B*255, b.B*255, Blend)
		);
	}
	public static byte Lerp(float a, float b, float Blend)
	{
		return (byte)(a * (1 - Blend) + b * Blend);
	}
}
