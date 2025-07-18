extends Range

@export var label : Label
@export var idx : int = 0
@export var mult : float = 1

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	value = AudioServer.get_bus_volume_linear(idx)
	value_changed.connect(func (_value):
		update()
	)
	update()

func update():
	AudioServer.set_bus_volume_linear(idx, value)
	label.text = str(roundi(value * mult))
