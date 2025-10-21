using Godot;
using System.Collections.Generic;

public partial class NPATerrainGenrator : Node
{
	public const int TERRAIN_CHUNK_SEGMENTS = 20;
	public const int TERRAIN_POINTS_DISTANCE = 64;
	public const float TERRAIN_NOISE_MAX_HEIGHT = 800;
	public const float TERRAIN_FLOOR_HEIGHT = 528;
	public const float PANCAKE_FILL_PERCENT = 0.3f;

	[Export]
	public Texture2D TerrainTexture;
	[Export]
	public FastNoiseLite Noise;
	[Export]
	public FastNoiseLite PancakeNoise;
	[Export]
	public PackedScene PancakePrefab;
	[Export]
	public NPAController Controller;

	public override void _Ready()
	{
		for (int i = 0; i < 200; i++)
		{
			float position = i * TERRAIN_CHUNK_SEGMENTS * TERRAIN_POINTS_DISTANCE;
			Node2D Mesh = CreateChunk(position);
			Mesh.Position = new Vector2(position, 0);
			AddChild(Mesh);
		}
	}

	public float GetTerrainHeight(float Position, bool Normalized = false)
	{
		if (Normalized)
			return -Noise.GetNoise1D(Position);
		else
			return -Noise.GetNoise1D(Position) * TERRAIN_NOISE_MAX_HEIGHT;
	}

	private StaticBody2D CreateChunk(float ChunckPosition)
	{
		StaticBody2D Body = new();
		// ---------------------------------------------------------Mesh
		MeshInstance2D MeshInstance = new();
		ArrayMesh ChunkMesh = new();
		List<Vector3> ChunkVertices = [];
		List<Vector2> ChunkUVs = [];

		for (int i = 0; i < TERRAIN_CHUNK_SEGMENTS; i++)
		{
			float point1 = i * TERRAIN_POINTS_DISTANCE;
			float point2 = (i + 1) * TERRAIN_POINTS_DISTANCE;
			float noised_point1 = GetTerrainHeight(ChunckPosition + point1);
			float noised_point2 = GetTerrainHeight(ChunckPosition + point2);
			
			ChunkVertices.Add(new Vector3(point1, noised_point1, 0));
			ChunkVertices.Add(new Vector3(point2, noised_point2, 0));
			ChunkVertices.Add(new Vector3(point1, TERRAIN_FLOOR_HEIGHT + noised_point1, 0));
			
			ChunkVertices.Add(new Vector3(point2, noised_point2, 0));
			ChunkVertices.Add(new Vector3(point2, TERRAIN_FLOOR_HEIGHT + noised_point2, 0));
			ChunkVertices.Add(new Vector3(point1, TERRAIN_FLOOR_HEIGHT + noised_point1, 0));

			ChunkUVs.Add(new Vector2(0, 0));
			ChunkUVs.Add(new Vector2(1, 0));
			ChunkUVs.Add(new Vector2(0, 1));
			
			ChunkUVs.Add(new Vector2(1, 0));
			ChunkUVs.Add(new Vector2(1, 1));
			ChunkUVs.Add(new Vector2(0, 1));

			float PancakePos = point1 + ChunckPosition;
			if (PancakeNoise.GetNoise1D(PancakePos) > PANCAKE_FILL_PERCENT)
			{
				Area2D Pancake = PancakePrefab.Instantiate() as Area2D;
				Pancake.Position = new(PancakePos, noised_point1 - 100);
				Pancake.BodyEntered += Body =>
				{
					Controller.PancakeCollected();
					Pancake.QueueFree();
				};
				AddChild(Pancake);
			}
		}

		Godot.Collections.Array MeshData = [];
		MeshData.Resize((int)Mesh.ArrayType.Max);
		MeshData[(int)Mesh.ArrayType.Vertex] = ChunkVertices.ToArray();
		MeshData[(int)Mesh.ArrayType.TexUV] = ChunkUVs.ToArray();

		ChunkMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, MeshData);
		MeshInstance.Mesh = ChunkMesh;
		MeshInstance.Texture = TerrainTexture;
		MeshInstance.TextureFilter = CanvasItem.TextureFilterEnum.Nearest;

		Body.AddChild(MeshInstance);
		//------------------------------------------------------------Collision
		CollisionPolygon2D CollisionPolygon = new();
		List<Vector2> PolygonVertices = [];

		for (int i = 0; i < TERRAIN_CHUNK_SEGMENTS; i++)
		{
			float point = i * TERRAIN_POINTS_DISTANCE;
			PolygonVertices.Add(new Vector2(point, GetTerrainHeight(ChunckPosition + point)));
		}
		float c_pos = TERRAIN_CHUNK_SEGMENTS * TERRAIN_POINTS_DISTANCE;
		PolygonVertices.Add(new Vector2(c_pos, GetTerrainHeight(ChunckPosition + c_pos)));
		PolygonVertices.Add(new Vector2(c_pos, TERRAIN_FLOOR_HEIGHT));
		PolygonVertices.Add(new Vector2(0, TERRAIN_FLOOR_HEIGHT + 200));

		CollisionPolygon.BuildMode = CollisionPolygon2D.BuildModeEnum.Segments;
		CollisionPolygon.Polygon = PolygonVertices.ToArray();

		Body.AddChild(CollisionPolygon);
		//-------------------------------------------------------------

		return Body;
	}
}
