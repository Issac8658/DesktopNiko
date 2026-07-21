using Godot;
using System;

public partial class SkinPreviewCreator : Node2D
{
	[Export]
	public Control Chupep; // area where to scatter nodes
	[Export]
	public PipeBuilder PipeBuilder;
	[Export]
	public PackedScene SkinPreviewPrefab;

	private NikoSkinManager SkinManager;

	public override void _Ready()
	{
		SkinManager = GetNode("/root/NikoSkinManager") as NikoSkinManager;

		NikoSkinManager.Skin[] SkinsList = SkinManager.GetSkinsList();

		Rect2I usableZone = DisplayServer.ScreenGetUsableRect(DisplayServer.GetPrimaryScreen());
		int usableZoneArea = usableZone.Size.X * usableZone.Size.Y;
		// TODO: replace this pls sfg;lkj'dsgfjlk;dfsgljk;dfsh;gkljdshf;lgjk мямямя
		usableZoneArea -= 440 * 300;
		usableZoneArea *= 5;
		usableZoneArea /= 10;

        Node2D page = new();
        AddChild(page);
		int areaSum = 0;

		foreach (NikoSkinManager.Skin skin in SkinsList)
		{
			if (areaSum >= usableZoneArea)
			{
				areaSum = 0;
        		page = new();
        		AddChild(page);
			}

			SkinPreview SkinNode = SkinPreviewPrefab.Instantiate() as SkinPreview;
			SkinNode.SkinImage = SkinManager.LoadSkinSprite(skin, "default");
			SkinNode.DisplayText = skin.Name;
			SkinNode.SkinScale = skin.Scale;
			SkinNode.OriginalSkinId = skin.Id;
			SkinNode.Position = new Vector2((float)GD.Randf() * Chupep.Size.X, (float)GD.Randf() * Chupep.Size.Y) - Chupep.Size / 2;
			page.AddChild(SkinNode);
			PipeBuilder.CreatePipe(SkinNode, page);
			areaSum += (int)(SkinNode.MonitorControl.Size.X * SkinNode.MonitorControl.Size.Y);
		}
	}
}

// sfsf