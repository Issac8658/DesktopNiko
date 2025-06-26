extends Window

var is_dragging : bool = false;
var mouse_offset : Vector2;

@export var cancel_button : Button
@export var PopupStreamPlayer : AudioStreamPlayer
@export var Sounds : Array[AudioStream]

func _ready() -> void:
	about_to_popup.connect(func ():
		PopupStreamPlayer.stream = Sounds[randi_range(0, len(Sounds)-1)]
		PopupStreamPlayer.play()
	)
	
	cancel_button.pressed.connect(func():
		visible = false
		GlobalControlls.facepick_update.emit()
	)


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
