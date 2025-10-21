extends Node

var is_pressed = false
var is_pressed_old = false

@export var mouse_down_sound : AudioStreamPlayer
@export var mouse_up_sound : AudioStreamPlayer

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float) -> void:
	if ValuesContainer.MouseSoundEnabled and not ValuesContainer.NikoHovered:
		is_pressed = Input.is_action_pressed("mouse_down")
		if is_pressed != is_pressed_old:
			if is_pressed == true:
				mouse_down_sound.play()
			else:
				mouse_up_sound.play()
		is_pressed_old = is_pressed
