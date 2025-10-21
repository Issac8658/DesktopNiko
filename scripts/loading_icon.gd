extends Window

@export var animator : AnimationPlayer

func _ready() -> void:
	GlobalController.SaveStarted.connect(func ():
		if ValuesContainer.ShowSavingIcon:
			position = get_target_position()
			visible = true
			animator.play("RESET")
			
	)
	GlobalController.SaveEnded.connect(func ():
		if ValuesContainer.ShowSavingIcon:
			animator.play("fade_out")
	)
	animator.animation_finished.connect(func (anim):
		if anim == "fade_out":
			visible = false
	)
	

func get_target_position():
	var target_screen = DisplayServer.window_get_current_screen(DisplayServer.MAIN_WINDOW_ID)
	return DisplayServer.screen_get_usable_rect(target_screen).end - Vector2i(112, 112)
