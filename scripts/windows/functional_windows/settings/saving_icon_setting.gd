extends CheckBox


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	button_pressed = ValuesContainer.ShowSavingIcon
	pressed.connect(func ():
		ValuesContainer.ShowSavingIcon = button_pressed
	)
