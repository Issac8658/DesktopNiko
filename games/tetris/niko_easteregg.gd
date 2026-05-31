extends Panel


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	gui_input.connect(func (event : InputEvent):
		if (event is InputEventMouseButton):
			if (event.button_index == MOUSE_BUTTON_LEFT):
				if (event.is_pressed()):
					modulate = Color.WHITE
					if not AchievementsController.IsAchievementTaked("everyone_is_dancing"):
						AchievementsController.TakeAchievement("everyone_is_dancing", true)
	)
