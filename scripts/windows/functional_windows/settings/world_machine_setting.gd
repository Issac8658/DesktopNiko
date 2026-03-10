extends CheckBox


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	button_pressed = ValuesContainer.IsWorldMachine
	pressed.connect(func ():
		ValuesContainer.IsWorldMachine = button_pressed
	)
	ValuesContainer.WorldMachineToggled.connect(func(toggled_on):
		button_pressed = toggled_on
	)
