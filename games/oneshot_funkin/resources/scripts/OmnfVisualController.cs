using Godot;
using System;
using System.Collections.Generic;

public partial class OmnfVisualController : Node
{
	private float _health = 50f;
	public float Health
	{
		get => _health;
		set
		{
			_health = value;
			_health = Mathf.Clamp(_health, 0f, 100f);
			UpdateHealthBar();
		}
	}

	[Export]
	public Control FillContainer;
	[Export]
	public Control DamageFill;
	[Export]
	public TextureRect NikoFacepicRect;
	[Export]
	public TextureRect WmFacepicRect;

	[ExportGroup("Facepics", "Facepic")]
	[Export]
	public string FacepicNikoDefault;
	[Export]
	public string FacepicNikoHigh;
	[Export]
	public string FacepicNikoLow;
	[Export]
	public string FacepicWMDefault;
	[Export]
	public string FacepicWMHigh;
	[Export]
	public string FacepicWMLow;
	[Export]
	public Label HealthLabel;

	[Export]
	public Vector3 CurrentCameraPosition;
	[Export]
	public Camera3D Camera;
	
	[ExportGroup("Labels", "Label")]
	[Export]
	public Label LabelMisses;
	[Export]
	public Label LabelAccuracy;
	public List<float> Accuracyes = [];

	public override void _Ready()
	{
		DamageFill.SetDeferred("Size", new Vector2(FillContainer.Size.X * (100f - _health) / 100f, DamageFill.Size.Y));
		NikoFacepicRect.Texture = (Texture2D)GetNode("/root/NikoSpritesModule").Call("get_sprite", FacepicNikoDefault);
		WmFacepicRect.Texture = (Texture2D)GetNode("/root/NikoSpritesModule").Call("get_sprite", FacepicWMDefault);
		HealthLabel.Text = Convert.ToString(_health);
	}

	public override void _Process(double delta)
	{
		Camera.Position = Camera.Position.Lerp(CurrentCameraPosition, 0.1f);
	}

	public void UpdateHealthBar()
	{
		DamageFill.Size = new Vector2(FillContainer.Size.X * (100f - _health) / 100f, DamageFill.Size.Y);

		if (_health >= 90)
		{
			NikoFacepicRect.Texture = (Texture2D)GetNode("/root/NikoSpritesModule").Call("get_sprite", FacepicNikoHigh);
			WmFacepicRect.Texture = (Texture2D)GetNode("/root/NikoSpritesModule").Call("get_sprite", FacepicWMLow);
		}
		else if (_health <= 10)
		{
			NikoFacepicRect.Texture = (Texture2D)GetNode("/root/NikoSpritesModule").Call("get_sprite", FacepicNikoLow);
			WmFacepicRect.Texture = (Texture2D)GetNode("/root/NikoSpritesModule").Call("get_sprite", FacepicWMHigh);
		}
		else
		{
			NikoFacepicRect.Texture = (Texture2D)GetNode("/root/NikoSpritesModule").Call("get_sprite", FacepicNikoDefault);
			WmFacepicRect.Texture = (Texture2D)GetNode("/root/NikoSpritesModule").Call("get_sprite", FacepicWMDefault);
		}
		HealthLabel.Text = Convert.ToString(_health);
	}

	public void ApplyAccuracy(float Accuracy)
	{
		Accuracyes.Add(Accuracy);
	}

}
