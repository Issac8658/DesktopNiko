using Godot;

[GlobalClass]
[Icon("res://sprites/icons/error.png")]
public partial class Event : Node
{
	[Signal]
	public delegate void EventEndedEventHandler();
	[Export]
	public Node EventController;

	public void StartEventController()
	{
		if (EventController != null)
		{
			if (EventController.HasMethod("do_event")) EventController.Call("do_event"); // GDScript
			else if (EventController.HasMethod("DoEvent")) EventController.Call("DoEvent"); // C#

			if (EventController.HasSignal("called_to_clear")) // GDScript
				EventController.Connect("called_to_clear", Callable.From(() => EmitSignal("EventEnded")));
			else if (EventController.HasSignal("CalledToClear")) // C#
				EventController.Connect("CalledToClear", Callable.From(() => EmitSignal("EventEnded")));
		}
	}
}
