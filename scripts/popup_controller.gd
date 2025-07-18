extends "res://scripts/window_controller.gd"

@export var cancel_button : Button
@export var PopupStreamPlayer : AudioStreamPlayer
@export var Sounds : Array[AudioStream]

func _ready() -> void:
	super()
	about_to_popup.connect(func ():
		PopupStreamPlayer.stream = Sounds[randi_range(0, len(Sounds)-1)]
		PopupStreamPlayer.play()
	)
	
	cancel_button.pressed.connect(func():
		visible = false
		GlobalControlls.facepick_update.emit()
	)
