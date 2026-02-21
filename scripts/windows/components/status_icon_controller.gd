extends Window

@export var gaming_mode_checkbox : CheckBox
@export var reset_pos_button : Button
@export var shutdown_button : Button

func _ready() -> void:
	ValuesContainer.GamingModeToggled.connect(func (toggled):
		gaming_mode_checkbox.button_pressed = toggled
	)
	shutdown_button.pressed.connect(func ():
		#GlobalController.quit_request.emit()
		visible = false
	)
	reset_pos_button.pressed.connect(func ():
		visible = false
		DisplayServer.window_set_position(GlobalController.GetDefaultPosition(DisplayServer.window_get_size(DisplayServer.MAIN_WINDOW_ID)))
	)
	gaming_mode_checkbox.toggled.connect(func (toggled):
		ValuesContainer.GamingModeEnabled = toggled
	)
