extends "res://scripts/windows/window_controller.gd"

signal yes_button_pressed()
signal no_button_pressed()

@export var cancel_button : Button
@export var accept_button : Button
@export var PopupStreamPlayer : AudioStreamPlayer
@export var Sounds : Array[AudioStream]

func _ready() -> void:
	super()
	about_to_popup.connect(func ():
		PopupStreamPlayer.stream = Sounds[randi_range(0, len(Sounds)-1)]
		PopupStreamPlayer.play()
	)
	
	cancel_button.pressed.connect(func():
		GlobalControlls.facepick_update.emit()
		no_button_pressed.emit()
		visible = false
	)
	
	accept_button.pressed.connect(func ():
		yes_button_pressed.emit()
		visible = false
	)
