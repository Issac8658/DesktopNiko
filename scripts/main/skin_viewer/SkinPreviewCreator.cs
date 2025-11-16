using Godot;
using System;

public partial class SkinPreviewCreator : Node
{
	[Export]
	public PackedScene SkinPreviewPrefab;

	private NikoSkinManager SkinManager;

	public override void _Ready()
	{
		SkinManager = GetNode("/root/NikoSkinManager") as NikoSkinManager;

		NikoSkinManager.Skin[] SkinsList = SkinManager.GetSkinsList();

		foreach (NikoSkinManager.Skin skin in SkinsList)
		{
			SkinPreview SkinNode = SkinPreviewPrefab.Instantiate() as SkinPreview;
			SkinNode.SkinImage = SkinManager.LoadSkinSprite(skin, "default");
			SkinNode.DisplayText = skin.Name;
			SkinNode.SkinScale = skin.Scale;
			SkinNode.OriginalSkinId = skin.Id;
			AddChild(SkinNode);
		}
	}
}
