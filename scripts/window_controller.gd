extends Window


var is_dragging : bool = false;
var mouse_offset : Vector2;

func MoveWindow(Offset:Vector2i) -> void:
	position += Vector2i(Offset);

func _on_topbar_gui_input(event: InputEvent) -> void:
	if event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_LEFT:
		if event.is_pressed():
			if get_visible_rect().has_point(event.position):
				is_dragging = true
				mouse_offset = event.position
		else:
			is_dragging = false
	if event is InputEventMouseMotion and is_dragging:
		MoveWindow(event.position - mouse_offset);

func _on_close_button_pressed() -> void:
	visible = false
