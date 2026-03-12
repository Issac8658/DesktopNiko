extends "res://scripts/windows/functional_windows/settings/windows_only_checkbox.gd"

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	button_pressed = ValuesContainer.ShowTaskbarIcon;
	toggled.connect(func (toggled_on):
		ValuesContainer.ShowTaskbarIcon = toggled_on;
	)
