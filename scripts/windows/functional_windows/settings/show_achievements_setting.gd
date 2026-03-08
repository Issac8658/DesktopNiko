extends CheckBox


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	button_pressed = ValuesContainer.ShowAchievements
	pressed.connect(func ():
		ValuesContainer.ShowAchievements = button_pressed
	)
