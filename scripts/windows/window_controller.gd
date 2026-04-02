extends Window

signal close_button_pressed();

@export var topbar : Control
@export var close_button : BaseButton
@export var hide_button : BaseButton
@export var ignore_on_top_settings : bool = false;

#var is_dragging : bool = false;
#var mouse_offset : Vector2;

func _ready() -> void:
	topbar.gui_input.connect(func (event : InputEvent):
		if event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_LEFT:
			if event.is_pressed():
				if get_visible_rect().has_point(event.position):
					DisplayServer.window_start_drag(get_window_id())
		#			is_dragging = true
		#			mouse_offset = event.position
		#	else:
		#		is_dragging = false
		#if event is InputEventMouseMotion and is_dragging:
		#	MoveWindow(event.position - mouse_offset)
	)
	
	close_button.pressed.connect(func ():
		close_button_pressed.emit()
		visible = false
	)
	close_requested.connect(func ():
		close_button_pressed.emit()
		visible = false
	)
	ValuesContainer.WindowsStateChanged.connect(func(AlwaysOnTop : bool):
		if not ignore_on_top_settings:
			always_on_top = AlwaysOnTop;
			Update()
	)

	if hide_button:
		hide_button.pressed.connect(func ():
			DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_MINIMIZED, get_window_id())
		)
	max_size = DisplayServer.screen_get_usable_rect(DisplayServer.SCREEN_PRIMARY).size

	if not ignore_on_top_settings:
		always_on_top = ValuesContainer.WindowsAlwaysOnTop;
		Update()

func MoveWindow(Offset:Vector2i) -> void:
	position += Vector2i(Offset);

func Update():
	if(visible):
		var pos := position;
		visible = false;
		visible = true;
		position = pos;
