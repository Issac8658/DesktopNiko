extends Window

@export var reset_pos_button : Button
@export var shutdown_button : Button

func _ready() -> void:
	shutdown_button.pressed.connect(func ():
		#GlobalController.quit_request.emit()
		visible = false
	)
	reset_pos_button.pressed.connect(func ():
		visible = false
		DisplayServer.window_set_position(GlobalController.GetDefaultPosition(DisplayServer.window_get_size(DisplayServer.MAIN_WINDOW_ID)), DisplayServer.MAIN_WINDOW_ID)
	)
