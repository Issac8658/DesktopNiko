extends Window

signal close_button_pressed();

@export var topbar : Control
@export var CloseButton : BaseButton
@export var HideButton : BaseButton

var is_dragging : bool = false;
var mouse_offset : Vector2;

func _ready() -> void:
	topbar.gui_input.connect(func (event : InputEvent):
		if event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_LEFT:
			if event.is_pressed():
				if get_visible_rect().has_point(event.position):
					is_dragging = true
					mouse_offset = event.position
			else:
				is_dragging = false
		if event is InputEventMouseMotion and is_dragging:
			MoveWindow(event.position - mouse_offset)
	)
	
	CloseButton.pressed.connect(func ():
		close_button_pressed.emit()
		visible = false
	)
	close_requested.connect(func ():
		close_button_pressed.emit()
		visible = false
	)
	if HideButton:
		HideButton.pressed.connect(func ():
			mode = MODE_MINIMIZED
		)
	max_size = DisplayServer.screen_get_usable_rect(DisplayServer.SCREEN_PRIMARY).size

func MoveWindow(Offset:Vector2i) -> void:
	position += Vector2i(Offset);
