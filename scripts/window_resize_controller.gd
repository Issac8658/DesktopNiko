extends Control

enum Direction { TopLeft, Top, TopRight, Right, BottomRight, Bottom, BottomLeft, Left }

@onready var window : Window = get_parent()
var last_offset : Vector2
var is_dragging = false

func _ready() -> void:
	$TopLeft.gui_input.connect(func (input): on_any_resize_zone_input(input, Direction.TopLeft))
	$Top.gui_input.connect(func (input): on_any_resize_zone_input(input, Direction.Top))
	$TopRight.gui_input.connect(func (input): on_any_resize_zone_input(input, Direction.TopRight))
	$Right.gui_input.connect(func (input): on_any_resize_zone_input(input, Direction.Right))
	$BottomRight.gui_input.connect(func (input): on_any_resize_zone_input(input, Direction.BottomRight))
	$Bottom.gui_input.connect(func (input): on_any_resize_zone_input(input, Direction.Bottom))
	$BottomLeft.gui_input.connect(func (input): on_any_resize_zone_input(input, Direction.BottomLeft))
	$Left.gui_input.connect(func (input): on_any_resize_zone_input(input, Direction.Left))

func resize(size_delta: Vector2i, direction: Direction):
	match direction:
		Direction.TopLeft:
			window.position.x += window.size.x - win_size_clamp_x(window.size.x - size_delta.x)
			window.size.x -= size_delta.x
			window.position.y += window.size.y - win_size_clamp_y(window.size.y - size_delta.y)
			window.size.y -= size_delta.y
		Direction.Top:
			window.position.y += window.size.y - win_size_clamp_y(window.size.y - size_delta.y)
			window.size.y -= size_delta.y
		Direction.TopRight:
			window.position.y += window.size.y - win_size_clamp_y(window.size.y - size_delta.y)
			window.size.y -= size_delta.y
			window.size.x += size_delta.x
		Direction.Right:
			window.size.x += size_delta.x
		Direction.BottomRight:
			window.size += size_delta
		Direction.Bottom:
			window.size.y += size_delta.y
		Direction.BottomLeft:
			window.position.x += window.size.x - win_size_clamp_x(window.size.x - size_delta.x)
			window.size.x -= size_delta.x
			window.size.y += size_delta.y
		Direction.Left:
			window.position.x += window.size.x - win_size_clamp_x(window.size.x - size_delta.x)
			window.size.x -= size_delta.x


func on_any_resize_zone_input(event, direction: Direction):
	if event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_LEFT:
		if event.is_pressed():
			#if get_visible_rect().has_point(event.position):
			is_dragging = true
			last_offset = event.position
		else:
			is_dragging = false
	if event is InputEventMouseMotion and is_dragging:
		resize(Vector2i(event.position - last_offset), direction);


func win_size_clamp_x(size : int):
	return clampi(size, window.min_size.x, window.max_size.x)
func win_size_clamp_y(size : int):
	return clampi(size, window.min_size.y, window.max_size.y)
