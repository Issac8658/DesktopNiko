extends PopupMenu

func _ready() -> void:
	GlobalControlls.gaming_mode_changed.connect(func (state):
			set_item_checked(2, state)
	)

func _on_id_pressed(id: int) -> void: # on any tray popup button pressed
	match id:
		0: # close
			GlobalControlls.quit_request.emit()
		1: # reset pos
			DisplayServer.window_set_position(GlobalControlls.get_default_pos(), DisplayServer.MAIN_WINDOW_ID)
		2: # gaming mode
			set_item_checked(2, not is_item_checked(2))
			GlobalControlls.set_gaming_mode(is_item_checked(2))
