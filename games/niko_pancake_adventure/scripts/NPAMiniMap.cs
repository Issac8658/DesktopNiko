using Godot;
using System;
public partial class NPAMiniMap : Control
{
	const int MAP_RESOLUTION = 500;
	const float MAP_SCALE = 2f;
	const float TERRAIN_SCALE = NPATerrainGenrator.TERRAIN_POINTS_DISTANCE;
	[Export]
	public NPATerrainGenrator TerrainGenrator;
	[Export]
	public Node2D Niko;
	public double time = 0f;

	public override void _Draw()
	{
		for (int i = 0; i < MAP_RESOLUTION; i++)
		{
			float pos1 = (float)i / MAP_RESOLUTION * Size.X;
			float pos2 = (float)(i + 1) / MAP_RESOLUTION * Size.X;

			float height1 = (TerrainGenrator.GetTerrainHeight(pos1 + Niko.Position.X, true) + 1f) * Size.Y / 2f;
			float height2 = (TerrainGenrator.GetTerrainHeight(pos2 + Niko.Position.X, true) + 1f) * Size.Y / 2f;

			DrawLine(new Vector2(pos1, height1), new Vector2(pos2, height2), new(1, 1, 1));
		}
		DrawLine(new Vector2(Size.X/2, 0), new Vector2(Size.X/2, Size.Y), new(1, 1, 1));
	}
	public override void _Process(double delta)
	{
		time += delta;
		QueueRedraw();
	}
}
