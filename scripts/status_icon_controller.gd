extends PopupMenu

func _on_id_pressed(id: int) -> void:
	match id:
		0: # close
			GlobalControlls.try_quit()
		1: # reset pos
			DisplayServer.window_set_position(GlobalControlls.get_default_pos(), DisplayServer.MAIN_WINDOW_ID)
		2: # gaming mode
			set_item_checked(2, not is_item_checked(2))
			GlobalControlls.set_gaming_mode(is_item_checked(2))
