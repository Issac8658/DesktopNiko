using Godot;
using System;

public partial class OMNFVisualController : Node
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
	public TextureRect NikoFacepickRect;
	[Export]
	public TextureRect WmFacepickRect;

	[ExportGroup("Facepicks", "Facepick")]
	[Export]
	public string FacepickNikoDefault;
	[Export]
	public string FacepickNikoHigh;
	[Export]
	public string FacepickNikoLow;
	[Export]
	public string FacepickWMDefault;
	[Export]
	public string FacepickWMHigh;
	[Export]
	public string FacepickWMLow;
	[Export]
	public Label HealthLabel;

	[Export]
	public Vector3 CurrentCameraPosition;
	[Export]
	public Camera3D Camera;

	public override void _Ready()
	{
		DamageFill.SetDeferred("Size", new Vector2(FillContainer.Size.X * (100f - _health) / 100f, DamageFill.Size.Y));
		NikoFacepickRect.Texture = (Texture2D)GetNode("/root/NikoSpritesModule").Call("get_sprite", FacepickNikoDefault);
		WmFacepickRect.Texture = (Texture2D)GetNode("/root/NikoSpritesModule").Call("get_sprite", FacepickWMDefault);
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
			NikoFacepickRect.Texture = (Texture2D)GetNode("/root/NikoSpritesModule").Call("get_sprite", FacepickNikoHigh);
			WmFacepickRect.Texture = (Texture2D)GetNode("/root/NikoSpritesModule").Call("get_sprite", FacepickWMLow);
		}
		else if (_health <= 10)
		{
			NikoFacepickRect.Texture = (Texture2D)GetNode("/root/NikoSpritesModule").Call("get_sprite", FacepickNikoLow);
			WmFacepickRect.Texture = (Texture2D)GetNode("/root/NikoSpritesModule").Call("get_sprite", FacepickWMHigh);
		}
		else
		{
			NikoFacepickRect.Texture = (Texture2D)GetNode("/root/NikoSpritesModule").Call("get_sprite", FacepickNikoDefault);
			WmFacepickRect.Texture = (Texture2D)GetNode("/root/NikoSpritesModule").Call("get_sprite", FacepickWMDefault);
		}
		HealthLabel.Text = Convert.ToString(_health);
	}

}
