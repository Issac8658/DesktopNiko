using Godot;
using System;
using System.Collections.Generic;

public partial class TetrisTileMap : Control
{
	private static readonly TileSet.CellNeighbor[] SQUARE_SIDES = [
		TileSet.CellNeighbor.TopLeftCorner,
		TileSet.CellNeighbor.TopSide,
		TileSet.CellNeighbor.TopRightCorner,
		TileSet.CellNeighbor.LeftSide,
		TileSet.CellNeighbor.RightSide,
		TileSet.CellNeighbor.BottomLeftCorner,
		TileSet.CellNeighbor.BottomSide,
		TileSet.CellNeighbor.BottomRightCorner,
	];
	private static readonly Vector2I[] SQUARE_DIRECTIONS = [
		new(-1, -1),
		new(0, -1),
		new(1, -1),
		new(-1, 0),
		new(1, 0),
		new(-1, 1),
		new(0, 1),
		new(1, 1),
	];

	public List<List<Rect2?>> Tiles = [];

	[Export]
	public TileSet TetrisTileSet;
	[Export]
	public TetrisGameController GameController;

	private TileSetAtlasSource _tilesSource;

	public override void _Ready()
	{
		_tilesSource = TetrisTileSet.GetSource(0) as TileSetAtlasSource;
		GameController.ReDrawTiles += FullRedraw;
		GameController.ReDrawZone += ReDrawZone;
	}

	public override void _Draw()
	{
		for (int X = 0; X < TetrisGameController.GAME_WIDTH; X++)
			for (int Y = 0; Y < TetrisGameController.GAME_HEIGHT; Y++)
			{
				Color BlockColor = GameController.Blocks[X][Y] != null ? TetrisGameController.GetColorFromType((TetrisGameController.BlockType)GameController.Blocks[X][Y]) : GameController.CurrentFigureIsHere(new(X, Y)) ? TetrisGameController.GetColorFromType(GameController.CurrentFigureType) : Colors.Black; // 😭🙏
				if (Tiles[X][Y] != null)
					DrawTextureRectRegion(_tilesSource.Texture, new(X * 40, Y * 40, 40, 40), (Rect2I)Tiles[X][Y], BlockColor);
				}
	}

	public void FullRedraw(){
		Tiles = [];
		for (int X = 0; X < TetrisGameController.GAME_WIDTH; X++)
		{
			List<Rect2?> Line = [];
			for (int Y = 0; Y < TetrisGameController.GAME_HEIGHT; Y++)
				if (GameController.Blocks[X][Y] != null || GameController.CurrentFigureIsHere(new (X, Y)))
					Line.Add(GetTile(GetSides(new(X,Y))));
				else
					Line.Add(null);
			Tiles.Add(Line);
		}
		QueueRedraw();
	}
	public void ReDrawZone(Rect2I Zone)
	{
		for (int X = Mathf.Clamp(Zone.Position.X, 0, TetrisGameController.GAME_WIDTH); X < Zone.Position.X + Zone.Size.X && X < TetrisGameController.GAME_WIDTH; X++)
		{
			for (int Y = Mathf.Clamp(Zone.Position.Y, 0, TetrisGameController.GAME_HEIGHT); Y < Zone.Position.Y + Zone.Size.Y && Y < TetrisGameController.GAME_HEIGHT; Y++)
				if (GameController.Blocks[X][Y] != null || GameController.CurrentFigureIsHere(new (X, Y)))
					Tiles[X][Y] = GetTile(GetSides(new(X,Y)));
				else
					Tiles[X][Y] = null;
		}
		QueueRedraw();
	}
	private bool[] GetSides(Vector2I BlockPos, bool WallsIsBlocks = false)
	{
		bool[] sides = [false, false, false, false, false, false, false, false];
	   
		TetrisGameController.BlockType? CurrentBlockType = GameController.Blocks[BlockPos.X][BlockPos.Y];
		CurrentBlockType = CurrentBlockType != null ? CurrentBlockType : GameController.CurrentFigureIsHere(BlockPos) ? GameController.CurrentFigureType : null;

		for (int side = 0; side < 8; side++)
		{
			Vector2I offset = SQUARE_DIRECTIONS[side];
			Vector2I offsetedPos = BlockPos + offset;
			if (offsetedPos.Y >= 0 && offsetedPos.X >= 0 && offsetedPos.Y < TetrisGameController.GAME_HEIGHT && offsetedPos.X < TetrisGameController.GAME_WIDTH)
			{
				TetrisGameController.BlockType? OtherBlockType = GameController.Blocks[offsetedPos.X][offsetedPos.Y];
				OtherBlockType = OtherBlockType != null ? OtherBlockType : GameController.CurrentFigureIsHere(new (offsetedPos.X, offsetedPos.Y)) ? GameController.CurrentFigureType : null;
				sides[side] = OtherBlockType != null && OtherBlockType == CurrentBlockType;
			}
			else sides[side] = WallsIsBlocks;
		}
		return sides;
	}
	private Rect2 GetTile(bool[] sides)
	{
		for (int i = 0; i < _tilesSource.GetTilesCount(); i++)
		{
			Vector2I tileId = _tilesSource.GetTileId(i);

			TileData data = _tilesSource.GetTileData(tileId, 0);

			bool canUse = true;
			for (int side = 0; side < 8; side++)
			{
				bool niceSide = data.GetTerrainPeeringBit(SQUARE_SIDES[side]) != -1 == sides[side];
				if (!niceSide){
					canUse = false;
					break;
				}
			}
			
			if (canUse)
				return _tilesSource.GetTileTextureRegion(tileId);
		}
		return new(0, 0, 20, 20);
	}

	public Vector2I GamePosToCurrentFigurePos(Vector2I GamePos) => GamePos - GameController.GetCurrentFigureRealPosition();
}
