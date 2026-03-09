using Godot;
using System;

public partial class MainScreenController : Node2D
{
	[Export]
	public Node MonitorsContainer;

	[Export]
	public TextureRect SkinPreviewImage;
	[Export]
	public Label NameLabel;
	[Export]
	public Label DescLabel;
	[Export]
	public Label AuthorLabel;
	[Export]
	public Label CommentsLabel;
	[Export]
	public Button ApplySkinButton;

	private SkinPreview[] SkinPreviews = [];
	private NikoSkinManager SkinManager;
	private string SelectedSkin = "";

	public override void _Ready()
	{
		SkinManager = GetNode("/root/NikoSkinManager") as NikoSkinManager;

		MonitorsContainer.ChildEnteredTree += Child =>
		{
			if (Child is SkinPreview Monitor)
				ConnectMonitor(Monitor);
		};
		foreach (Node node in MonitorsContainer.GetChildren())
		{
			if (node is SkinPreview Monitor)
				ConnectMonitor(Monitor);
		}
		ApplySkinButton.Pressed += () =>
		{
			if (SelectedSkin != "")
				SkinManager.SetSkin(SelectedSkin);
		};
	}
	

	private void ConnectMonitor(SkinPreview Monitor)
	{
		NikoSkinManager.Skin? skin = SkinManager.GetSkinFromId(Monitor.OriginalSkinId);
		if (skin is NikoSkinManager.Skin trueSkin)
		{
			Node Container = Monitor.GetChild(0);
			if (Container != null)
			{
				if (Container is Control MonitorContainer){
					bool isDragging = false;
					bool isMousePressed = false;

					Vector2 lastVec = new();
					MonitorContainer.GuiInput += Event =>
					{
						if (Event is InputEventMouseButton MouseButton)
						{
							isMousePressed = Event.IsPressed();
							//GD.Print(isMousePressed);
							if (!isMousePressed)
							{
								if (!isDragging)
								{
									Texture2D sprite = SkinManager.LoadSkinSprite(trueSkin, "default");
									NameLabel.Text = trueSkin.Name;
									DescLabel.Text = trueSkin.Description;
									AuthorLabel.Text = trueSkin.Author;
									CommentsLabel.Text = trueSkin.Comment;
									SkinPreviewImage.Texture = sprite;
									SkinPreviewImage.CustomMinimumSize = sprite.GetSize() * trueSkin.Scale;
									SelectedSkin = trueSkin.Id;
								}
							}
							else
								isDragging = false;
							lastVec = MouseButton.GlobalPosition;
						}
						else if (Event is InputEventMouseMotion MouseMotion)
						{
							isDragging = lastVec.DistanceTo(MouseMotion.GlobalPosition) > 10;
						}
					};
				}
			}
		}
	}
}
