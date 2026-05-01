using Godot;

public partial class EventsManager : Node
{
	private bool _eventing = false;
	[Export]
	public Godot.Collections.Array<PackedEvent> Events = [];

	private ValuesContainer _valuesContainer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_valuesContainer = GetNode<ValuesContainer>("/root/ValuesContainer");

		Godot.Collections.Array<PackedEvent> SortedEvents = [];
		foreach(PackedEvent packedEvent in Events)
		{
			AddEventSorted(SortedEvents, packedEvent);
		}

		_valuesContainer.Clicked += diff =>
		{
			if (_valuesContainer.DoEvents && !_eventing)
			{
				foreach(PackedEvent packedEvent in SortedEvents)
				{
					float rand = GD.Randf() * 100f;
					if (rand <= packedEvent.SpawnChance && _valuesContainer.Clicks >= packedEvent.MinClicks && _valuesContainer.CPS <= packedEvent.MaxCPS)
					{
						Event EventNode = packedEvent.LinkedScene.Instantiate<Event>();
						AddChild(EventNode);
						_eventing = true;
						EventNode.StartEventController();
						GD.Print($"Event {Events.IndexOf(packedEvent)}:{EventNode.Name} c{rand}");
						EventNode.EventEnded += () =>
						{
							EventNode.QueueFree();
							_eventing = false;
						};
						break;
					}
				}
			}
		};
	}

	public static void AddEventSorted(Godot.Collections.Array<PackedEvent> list, PackedEvent value)
	{
		if (list.Count == 0)
		{
			list.Add(value);
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (i == list.Count - 1)
			{
				list.Add(value);
				i++;
			}
			else if(list[i].SpawnChance > value.SpawnChance && list[i + 1].SpawnChance <= value.SpawnChance )
			{
				list.Insert(i + 1, value);
				return;
			}
		}
	}
}
