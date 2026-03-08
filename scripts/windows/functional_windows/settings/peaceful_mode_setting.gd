extends CheckBox


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	button_pressed = ValuesContainer.PeacfulMode
	toggled.connect(func (toggled_on):
		ValuesContainer.PeacfulMode = toggled_on
	)
