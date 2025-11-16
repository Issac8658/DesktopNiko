using Godot;
using System.Collections.Generic;

public partial class GlobalController : Node
{
    [Signal] public delegate void SaveStartedEventHandler();
    [Signal] public delegate void SaveEndedEventHandler();
    public List<Node> ScriptsToSave = [];
    private ValuesContainer Values;

    public override void _Ready()
    {
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

    public void Save()
    {
        EmitSignal("SaveStarted");
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
        Vector2I ResultPos = ScreenPos + DisplayServer.ScreenGetSize(PrimaryScreenId) / 2 + NikoSize / 2;
        return ResultPos;
    }
}
