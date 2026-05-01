using Godot;

[GlobalClass]
public partial class PackedEvent : Resource
{
    [Export]
    public float SpawnChance = 0.01f;
    [Export]
    public ulong MinClicks = 100;
    [Export]
    public uint MaxCPS = 100;

    [Export]
    public string DisplayName = "Unnamed Event";

    [Export]
    public PackedScene LinkedScene;
}