using Godot;
using System;

public partial class NextFigureDrawer : Control
{
	private readonly TileSet.CellNeighbor[] SQUARE_SIDES = [
		TileSet.CellNeighbor.TopLeftCorner,
		TileSet.CellNeighbor.TopSide,
		TileSet.CellNeighbor.TopRightCorner,
		TileSet.CellNeighbor.LeftSide,
		TileSet.CellNeighbor.RightSide,
		TileSet.CellNeighbor.BottomLeftCorner,
		TileSet.CellNeighbor.BottomSide,
		TileSet.CellNeighbor.BottomRightCorner,
	];
	private readonly Vector2I[] SQUARE_DIRECTIONS = [
		new(-1, -1),
		new(0, -1),
		new(1, -1),
		new(-1, 0),
		new(1, 0),
		new(-1, 1),
		new(0, 1),
		new(1, 1),
	];

	[Export]
	public TileSet TetrisTileSet;
	[Export]
	public TetrisGameController GameController;
	[Export]
	public int NextFigureId = 0;

	private TileSetAtlasSource _tilesSource;

	public override void _Ready()
	{
		_tilesSource = TetrisTileSet.GetSource(0) as TileSetAtlasSource;
		GameController.ReDrawNextTiles += QueueRedraw;
	}

	public override void _Draw()
	{
		bool[][] figure = GameController.NextFigures[NextFigureId];
		for (int X = 0; X < figure.Length; X++)
			for (int Y = 0; Y < figure[0].Length; Y++)
				if (figure[X][Y])
					DrawTextureRectRegion(_tilesSource.Texture, new(X * 40, Y * 40, 40, 40), GetTile(GetSidesFigure(new(X,Y), figure)));
		CustomMinimumSize = new(figure.Length * 40f, figure[0].Length * 40f);
	}

	private bool[] GetSidesFigure(Vector2I BlockPos, bool[][] Map, bool WallsIsBlocks = false)
	{
		bool[] sides = [false, false, false, false, false, false, false, false];
	   
		for (int side = 0; side < 8; side++)
		{
			Vector2I offset = SQUARE_DIRECTIONS[side];
			Vector2I offsetedPos = BlockPos + offset;
			if (offsetedPos.Y >= 0 && offsetedPos.X >= 0 && offsetedPos.Y < Map[0].Length && offsetedPos.X < Map.Length)
			{
				if (Map[offsetedPos.X][offsetedPos.Y])
					sides[side] = true;
			}
			else sides[side] = WallsIsBlocks;
		}
		return sides;
	}
	private Rect2 GetTile(bool[] sides)
	{
		Rect2 mostSimilarTile = new(0, 0, 20, 20);
		int similarSidesCount = 0;
		for (int i = 0; i < _tilesSource.GetTilesCount(); i++)
		{
			Vector2I tileId = _tilesSource.GetTileId(i);

			TileData data = _tilesSource.GetTileData(tileId, 0);

			bool canUse = true;
			int NiceSidesCount = 0;
			for (int side = 0; side < 8; side++)
			{
				bool niceSide = data.GetTerrainPeeringBit(SQUARE_SIDES[side]) != -1 == sides[side];
				if (niceSide)
					NiceSidesCount += 1;
				else {
					canUse = false;
				};
			}
			
			if (canUse)
				return _tilesSource.GetTileTextureRegion(tileId);
			
			if (NiceSidesCount > similarSidesCount)
			{
				mostSimilarTile = _tilesSource.GetTileTextureRegion(tileId);
				similarSidesCount = NiceSidesCount;
			}
			
			//data.GetTerrainPeeringBit(,)
			//tilesSource.GetTileTextureRegion(tileId);
		}
		return mostSimilarTile;
	}
}
