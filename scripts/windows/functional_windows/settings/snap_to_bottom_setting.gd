extends CheckBox


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	button_pressed = ValuesContainer.SnapToBottom
	pressed.connect(func ():
		ValuesContainer.SnapToBottom = button_pressed
	)
