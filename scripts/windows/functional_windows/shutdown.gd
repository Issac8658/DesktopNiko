extends Button

@export var shutdown_popup : WMPopup
@export var use_double_click : bool = true

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	mouse_entered.connect(func ():
		if not shutdown_popup.visible and not ValuesContainer.IsFacepicForced:
			ValuesContainer.ForceFacepic("sad")
	)
	mouse_exited.connect(func ():
		if not shutdown_popup.visible and ValuesContainer.ForcedFacepicId == "sad":
			ValuesContainer.UnforceFacepic()
	)
	gui_input.connect(func (event : InputEvent):
		if (event is InputEventMouseButton):
			if (event.pressed and (event.double_click or not use_double_click)):
				if not shutdown_popup.visible and ValuesContainer.ForcedFacepicId == "sad":
					ValuesContainer.ForceFacepic("cry")
				shutdown_popup.popup()
				shutdown_popup.grab_focus()
	)
	shutdown_popup.no_button_pressed.connect(func ():
		if (ValuesContainer.ForcedFacepicId == "cry"):
			ValuesContainer.UnforceFacepic() 
	)
	shutdown_popup.yes_button_pressed.connect(func ():
		GlobalController.Shutdown()
	)
	shutdown_popup.focus_exited.connect(func ():
		if shutdown_popup.visible:
			shutdown_popup.grab_focus()
	)
