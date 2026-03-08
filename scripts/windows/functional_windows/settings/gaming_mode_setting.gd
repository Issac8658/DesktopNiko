extends CheckBox


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	button_pressed = ValuesContainer.GamingModeEnabled
	toggled.connect(func (toggled_on):
		ValuesContainer.GamingModeEnabled = toggled_on
	)
	ValuesContainer.GamingModeToggled.connect(func (toggled_on):
		button_pressed = toggled_on
	)
