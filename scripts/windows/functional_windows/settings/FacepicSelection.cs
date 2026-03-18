using Godot;
using System;
using System.Collections.Generic;

public partial class FacepicSelection : Node
{
	[Export]
	public TextureRect PreviewImage;
	[Export]
	public OptionButton FacepicOption;
	[Export]
	public ValuesContainer.FacepicType TargetFacepic;

	private ValuesContainer _valuesContainer;
	private NikoSkinManager _skinManager;

	public static readonly List<string> AllowedSkinSprites = [
		NikoSkinManager.DEFAULT_SPRITE_ID,
		"normal",
		"speak",
		"shock",
		"surprised",
		"pancakes",
	];
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_valuesContainer = GetNode<ValuesContainer>("/root/ValuesContainer");
		_skinManager = GetNode<NikoSkinManager>("/root/NikoSkinManager");

		_skinManager.SkinChanged += SkinId =>
		{
			UpdateFacepic();
			UpdateVariants();
			GetWindow().Size = Vector2I.Zero;
		};
		FacepicOption.ItemSelected += ix =>
		{
			switch (TargetFacepic)
			{
				case ValuesContainer.FacepicType.Default:
					{
						_valuesContainer.IdleFacepic = FacepicOption.GetItemText((int)ix);
						break;
					}
				case ValuesContainer.FacepicType.Speak:
					{
						_valuesContainer.SpeakFacepic = FacepicOption.GetItemText((int)ix);
						break;
					}
				case ValuesContainer.FacepicType.Scared:
					{
						_valuesContainer.ScaredFacepic = FacepicOption.GetItemText((int)ix);
						break;
					}
				case ValuesContainer.FacepicType.ScaredSpeak:
					{
						_valuesContainer.ScaredSpeakFacepic = FacepicOption.GetItemText((int)ix);
						break;
					}
			}
			UpdateFacepic();
		};

		UpdateFacepic();
		UpdateVariants();
	}

	private void UpdateVariants()
	{
		FacepicOption.Clear();
		NikoSkinManager.Skin CurrentSkin = _skinManager.GetCurrentSkin();
		foreach (string SpritePair in AllowedSkinSprites)
		{
			FacepicOption.AddItem(SpritePair);
			//FacepicOption.SetItemIcon(FacepicOption.ItemCount - 1, _skinManager.GetCurrentSkinSprite(SpritePair));
		}
		FacepicOption.AddSeparator("settings.niko.extra_facepics");
		foreach (KeyValuePair<string, string> SpritePair in CurrentSkin.ExtraSprites)
		{
			FacepicOption.AddItem(SpritePair.Key);
			//FacepicOption.SetItemIcon(FacepicOption.ItemCount - 1, _skinManager.GetCurrentSkinSprite(SpritePair.Key));
		}

		FacepicOption.Select(-1);
		FacepicOption.Text = "???";
		for (int i = 0; i < FacepicOption.ItemCount; i++)
		{
			switch (TargetFacepic)
			{
				case ValuesContainer.FacepicType.Default:
					{
						if (FacepicOption.GetItemText(i) == _valuesContainer.IdleFacepic)
							FacepicOption.Select(i);
						break;
					}
				case ValuesContainer.FacepicType.Speak:
					{
						if (FacepicOption.GetItemText(i) == _valuesContainer.SpeakFacepic)
							FacepicOption.Select(i);
						break;
					}
				case ValuesContainer.FacepicType.Scared:
					{
						if (FacepicOption.GetItemText(i) == _valuesContainer.ScaredFacepic)
							FacepicOption.Select(i);
						break;
					}
				case ValuesContainer.FacepicType.ScaredSpeak:
					{
						if (FacepicOption.GetItemText(i) == _valuesContainer.ScaredSpeakFacepic)
							FacepicOption.Select(i);
						break;
					}
			}
		}
	}

	private void UpdateFacepic()
	{
		Texture2D Sprite = null;
		switch (TargetFacepic)
		{
			case ValuesContainer.FacepicType.Default:
				{
					Sprite = _skinManager.GetCurrentSkinSprite(_valuesContainer.IdleFacepic);
					break;
				}
			case ValuesContainer.FacepicType.Speak:
				{
					Sprite = _skinManager.GetCurrentSkinSprite(_valuesContainer.SpeakFacepic);
					break;
				}
			case ValuesContainer.FacepicType.Scared:
				{
					Sprite = _skinManager.GetCurrentSkinSprite(_valuesContainer.ScaredFacepic);
					break;
				}
			case ValuesContainer.FacepicType.ScaredSpeak:
				{
					Sprite = _skinManager.GetCurrentSkinSprite(_valuesContainer.ScaredSpeakFacepic);
					break;
				}
		}
		PreviewImage.Texture = Sprite;
		PreviewImage.CustomMinimumSize = Sprite.GetSize() * _skinManager.GetCurrentSkinBaseScale();
	}
}
