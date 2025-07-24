extends Node

@export var events : Array[Event]

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	GlobalControlls.clicked.connect(func ():
		for event in events:
			var rand = randf()
			if rand <= event.spawn_chance:
				if GlobalControlls.clicks >= event.min_clicks:
					print("Event " + str(events.find(event)) + ":" + event.name + " c" + str(rand))
					event.event()
					break
	)
