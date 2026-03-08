extends CheckBox


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	button_pressed = ValuesContainer.MouseSoundEnabled
	pressed.connect(func ():
		ValuesContainer.MouseSoundEnabled = button_pressed
	)
