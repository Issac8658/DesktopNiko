extends StatusIndicator

@export var styled_menu : PopupMenu
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pressed.connect(func (_mouse_button : int, mouse_pos:Vector2i):
		styled_menu.position = mouse_pos - styled_menu.size + Vector2i(20,20)
		styled_menu.visible = true
	)
	styled_menu.mouse_exited.connect(func ():
		styled_menu.visible = false
	)
