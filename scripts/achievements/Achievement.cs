using Godot;

[GlobalClass]
public partial class Achievement : Resource
{
	[Export]
	public string Id = "enter_your_achievement_id_here";
	[Export]
	public Texture2D Icon;
	[Export]
	public string Title = "Default achievement title";
	[Export]
	public string Description = "Default achievement description";
	[Export]
	public bool Hidden = false;
}
