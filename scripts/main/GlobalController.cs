using Godot;
using System.Collections.Generic;

public partial class GlobalController : Node
{
	
	[Signal] public delegate void SaveStartedEventHandler();
	[Signal] public delegate void SaveEndedEventHandler();
	public List<Node> ScriptsToSave = [];
	private ValuesContainer Values;

	private SaveLoad _saveLoadNode;
	private ValuesContainer _valuesContainer;

	public override void _Ready()
	{
		_saveLoadNode = GetNode<SaveLoad>("/root/SaveLoad");
		_valuesContainer = GetNode<ValuesContainer>("/root/ValuesContainer");

		_saveLoadNode.Load();

		GD.Print("Hello, Niko! :3"); // Hello!!1 :3
	}

	public override void _Notification(int what)
	{
		switch ((long)what)
		{
			case NotificationWMCloseRequest:
				Save();
				GD.Print("Goodbye, Niko!");
				GetTree().Quit();
				break;
		}
	}

    public override void _UnhandledInput(InputEvent @event)
    {
		if (@event is InputEventKey EventKey)
		{
			if (EventKey.Keycode == Key.F && EventKey.Pressed)
			{
				_valuesContainer.NikoIsFlipped = !_valuesContainer.NikoIsFlipped;
			}
		}
    }


	public void Save()
	{
		EmitSignal("SaveStarted");
		_saveLoadNode.Save();
		foreach (Node Script in ScriptsToSave)
		{
			if (Script.HasMethod("Save"))
			{
				Script.Call("Save");
			}
			if (Script.HasMethod("save"))
			{
				Script.Call("save");
			}
		}
		EmitSignal("SaveEnded");
	}
	public static Vector2I GetDefaultPosition(Vector2I NikoSize)
	{
		int PrimaryScreenId = DisplayServer.GetPrimaryScreen();
		Vector2I ScreenPos = DisplayServer.ScreenGetPosition(PrimaryScreenId);
		Vector2I ResultPos = ScreenPos + DisplayServer.ScreenGetSize(PrimaryScreenId) / 2 - NikoSize / 2;
		return ResultPos;
	}
}
