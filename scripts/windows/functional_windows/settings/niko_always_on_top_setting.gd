extends CheckBox

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	button_pressed = ValuesContainer.NikoAlwaysOnTop;
	toggled.connect(func (toggled_on):
		ValuesContainer.NikoAlwaysOnTop = toggled_on;
	)
