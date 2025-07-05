extends Window

@onready var primary_screen = DisplayServer.get_primary_screen()
@export var animator : AnimationPlayer

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	position = DisplayServer.screen_get_position(primary_screen) + DisplayServer.screen_get_size(primary_screen) - Vector2i(112, 156)
	
	GlobalControlls.saving.connect(func ():
		mouse_passthrough_polygon[1].x = 96.0
		mouse_passthrough_polygon[2].x = 96.0
		mouse_passthrough_polygon[2].y = 96.0
		mouse_passthrough_polygon[3].y = 96.0
		animator.play("RESET")
	)
	GlobalControlls.saved.connect(func ():
		animator.play("fade_out")
	)
	animator.animation_finished.connect(func (anim):
		if anim == "fade_out":
			mouse_passthrough_polygon[1].x = 0
			mouse_passthrough_polygon[2].x = 0
			mouse_passthrough_polygon[2].y = 0
			mouse_passthrough_polygon[3].y = 0
	)
