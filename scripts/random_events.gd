extends Node

var events : Array[Node]
@onready var niko_conroller = get_parent()

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	events = get_children()
	GlobalControlls.clicked.connect(func ():
		for event in events:
			var rand = randf() * 100
			if rand <= event.spawn_chance and GlobalControlls.clicks >= event.min_clicks and niko_conroller.cps <= event.max_cps:
					print("Event " + str(events.find(event)) + ":" + event.name + " c" + str(rand))
					event.event()
					GlobalControlls.event_started.emit(event.name)
					break
	)
