extends Node

@onready var niko_controller = get_parent()

@export var events : Array[PackedEvent]

var eventing := false

func events_sort(a : PackedEvent, b : PackedEvent):
	return a.spawn_chance < b.spawn_chance

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	events.sort_custom(events_sort) # to cancel the "overlap" of rare events by frequent ones
	ValuesContainer.Clicked.connect(func (_difference):
		if ValuesContainer.DoEvents && not eventing:
			for event : PackedEvent in events:
				var rand = randf() * 100
				if rand <= event.spawn_chance and ValuesContainer.Clicks >= event.min_clicks and ValuesContainer.CPS <= event.max_cps:
						var event_node : Event = event.linked_scene.instantiate()
						add_child(event_node)
						eventing = true
						event_node.event()
						#GlobalControlls.event_started.emit(event_node.name)
						print("Event " + str(events.find(event)) + ":" + event_node.name + " c" + str(rand))
						event_node.event_ended.connect(func():
							event_node.queue_free()
							eventing = false
						)
						break
	)
