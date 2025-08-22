extends Window

@export var gaming_mode_checkbox : CheckBox
@export var reset_pos_button : Button
@export var shutdown_button : Button

func _ready() -> void:
	GlobalControlls.gaming_mode_changed.connect(func ():
		gaming_mode_checkbox.button_pressed = GlobalControlls.gaming_mode_enabled
	)
	shutdown_button.pressed.connect(func ():
		GlobalControlls.quit_request.emit()
		visible = false
	)
	reset_pos_button.pressed.connect(func ():
		visible = false
		DisplayServer.window_set_position(GlobalControlls.get_default_pos(), DisplayServer.MAIN_WINDOW_ID)
	)
	gaming_mode_checkbox.toggled.connect(func (toggled):
		GlobalControlls.set_gaming_mode(toggled)
	)
