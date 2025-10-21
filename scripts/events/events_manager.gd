extends Node

@onready var niko_controller = get_parent()

@export var events : Array[PackedEvent]

func events_sort(a : PackedEvent, b : PackedEvent):
	if a.spawn_chance < b.spawn_chance:
		return true
	return false

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	events.sort_custom(events_sort) # to cancel the "overlap" of rare events by frequent ones
	ValuesContainer.Clicked.connect(func (_difference):
		if ValuesContainer.DoEvents:
			for event : PackedEvent in events:
				var rand = randf() * 100
				if rand <= event.spawn_chance and ValuesContainer.Clicks >= event.min_clicks and niko_controller.cps <= event.max_cps:
						var event_node : Event = event.linked_scene.instantiate()
						add_child(event_node)
						event_node.event()
						#GlobalControlls.event_started.emit(event_node.name)
						print("Event " + str(events.find(event)) + ":" + event_node.name + " c" + str(rand))
						event_node.event_ended.connect(event_node.queue_free)
						break
	)
