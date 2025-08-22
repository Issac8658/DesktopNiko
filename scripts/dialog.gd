extends Window

signal dialog_showed()
signal dialog_skiped()
#signal symbol_showed(symbol : String)

@export var text_sound : AudioStreamPlayer
@export var wm_sound : AudioStreamPlayer
@export var timer : Timer
@export var text_label : RichTextLabel
@export var rect : TextureRect

var parsed_text = ""
var target_showed_symbols : int = 0
var showed_symbols : int = 0
var _use_wm_sound : bool = false

func _ready() -> void:
	update_pos()

func _input(_event: InputEvent) -> void:
	if Input.is_action_pressed("dialog_skip"):
		skip_dialog()

func skip_dialog():
	if visible:
		visible = false
		if timer.is_connected("timeout", show_next_symbol):
			timer.disconnect("timeout", show_next_symbol)
			timer.stop()
		text_label.visible_characters = 0
		showed_symbols = 0
		dialog_skiped.emit()


func update_pos():
	var main_window_screen = DisplayServer.window_get_current_screen(DisplayServer.MAIN_WINDOW_ID)
	var screen_rect = DisplayServer.screen_get_usable_rect(main_window_screen)
	@warning_ignore("integer_division")
	position = screen_rect.end - Vector2i(screen_rect.size.x / 2 + size.x / 2, size.y + 8)


func show_dialog(text : String, icon: Texture2D, show_time : float = 0.05, use_wm_sound : bool = false):
	update_pos()
	if timer.is_connected("timeout", show_next_symbol):
		timer.disconnect("timeout", show_next_symbol)
		timer.stop()
	_use_wm_sound = use_wm_sound
	text_label.visible_characters = 0
	showed_symbols = 0
	text_label.text = text
	parsed_text = text_label.get_parsed_text()
	if icon:
		rect.texture = icon
		rect.visible = true
	else:
		rect.visible = false
	target_showed_symbols = text_label.get_total_character_count()
	visible = true
	timer.wait_time = show_time
	timer.timeout.connect(show_next_symbol)
	timer.start()
	dialog_showed.emit()
	
func show_next_symbol():
	text_label.visible_characters += 1
	showed_symbols += 1
	if not parsed_text[showed_symbols - 1] == " ":
		if _use_wm_sound:
			wm_sound.play()
		else:
			text_sound.play()
	if showed_symbols >= target_showed_symbols:
		if timer.is_connected("timeout", show_next_symbol):
			timer.disconnect("timeout", show_next_symbol)
			timer.stop()
