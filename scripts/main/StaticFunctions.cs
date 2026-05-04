using Godot;

public static class StaticFunctions
{
	private static SceneTree _sceneTree;
	public static Node CurrentScene
	{
		get
		{
			_sceneTree ??= (SceneTree)Engine.GetMainLoop();
			return _sceneTree.CurrentScene;
		}
	}

	public static int MainWindowId = (int)DisplayServer.MainWindowId;

	public static Vector2I V2ILerp(Vector2I a, Vector2I b, float t)
	{
		return (Vector2I)(a * new Vector2(1 - t, 1 - t) + new Vector2(t, t) * b);
	}
	
	public static Vector2I GetNikoPosition()
	{
		return DisplayServer.WindowGetPosition(MainWindowId);
	}
	
	public static Vector2 GetNikoSize()
	{
		return CurrentScene.GetNode<Control>("/root/TheWorldMachine/NikoBase/Niko").Size;
	}
}
