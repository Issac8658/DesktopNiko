extends Node

const main_window_id = DisplayServer.MAIN_WINDOW_ID

@export_range(0, 5, 0.1, "or_greater") var anim_duration = .2
@export_range(0, 50, 1, "or_greater") var scare_threshold = 10
@export var afk_time : float = 10.0

@export_group("resources")
@export var niko_sad : Texture2D
@export var meow_sounds : Array[AudioStream]

@export_group("objects")
@export var settings_window : Window
@export var niko_sound : AudioStreamPlayer
@export var animator : AnimationPlayer
@export var rect_for_anim : TextureRect
@export var settings_content : Control
@export var facepicks_container : Control
@export var meow_sound_option_button : OptionButton
@export var click_count_label : Label
@export var cps_counter : Label
@export var menu_label : Control

var mouse_offset : Vector2;
var is_dragging : bool = false;
var PositionBeforeMove : Vector2i;
var peaceful_mode = false
var in_anim = false
var exit_button_hover = false

var current_afk_time : float = 0.0
var timed_clicks : int = 0
var cps : int = 0
#var time = 0


func _ready() -> void:
	$SettingsWindow.visible = false
	GlobalControlls.gaming_mode_changed.connect(update_gaming_mode)
	GlobalControlls.idle_facepick_update.connect(func ():
		reset_face_pick()
	)
	
	niko_sound.stream = meow_sounds[GlobalControlls.current_meow_sound_id]
	meow_sound_option_button.selected = GlobalControlls.current_meow_sound_id
	
	click_count_label.text = str(GlobalControlls.clicks)
	reset_face_pick()


func _process(delta: float) -> void:
	current_afk_time += delta
	if current_afk_time >= afk_time:
		current_afk_time = 0
		animator.play("niko_look_around")



func update_gaming_mode(state: bool):
	$IndicatorPopup.set_item_checked(2, state)
	settings_content.get_node("GamingMode").button_pressed = state



# Niko functions
@warning_ignore("unused_parameter")
func _on_animation_finished(anim_name: StringName) -> void: #afk animation stop
	animator.stop()
	rect_for_anim.texture = GlobalControlls.idle_facepick


func _on_niko_input(event: InputEvent) -> void: # main Niko operations
	current_afk_time = 0.0
	if animator.is_playing():
		_on_animation_finished("") # cancel afk animation on any input
		
	if event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_LEFT:
		if event.is_pressed():
			niko_sound.play() # meow
			GlobalControlls.clicks += 1
			timed_clicks += 1
			click_count_label.text = str(GlobalControlls.clicks)
			
			DiscordRPC.start_timestamp = int(Time.get_unix_time_from_system())
			DiscordRPC.refresh()
			
			anim_niko() # :O
			
			is_dragging = true
			mouse_offset = event.position
		else:
			is_dragging = false
	if event is InputEventMouseMotion and is_dragging:
		move_window(event.position - mouse_offset);


func _input(event) -> void: # Niko flip
	if event.is_action_pressed("flip"):
		rect_for_anim.flip_h = not rect_for_anim.flip_h


func anim_niko(): # Niko facepick animation
	if peaceful_mode or cps < scare_threshold:
		rect_for_anim.texture = GlobalControlls.speak_facepick
	else:
		rect_for_anim.texture = GlobalControlls.scare_speak_facepick
		
	in_anim = true
	
	await get_tree().create_timer(anim_duration).timeout
	
	if peaceful_mode or cps < scare_threshold:
		rect_for_anim.texture = GlobalControlls.idle_facepick
	else:
		rect_for_anim.texture = GlobalControlls.scare_facepick
		
	in_anim = false


func reset_face_pick():
	if peaceful_mode or cps < scare_threshold:
		rect_for_anim.texture = GlobalControlls.idle_facepick
	else:
		rect_for_anim.texture = GlobalControlls.scare_facepick


func _on_meow_sound_selected(index: int) -> void:
	GlobalControlls.current_meow_sound_id = index
	niko_sound.stream = meow_sounds[index]



# Window functions
func _on_window_mouse_entered() -> void: # Niko menu show
	menu_label.visible = true


func _on_window_mouse_exited() -> void: # Niko menu hide
	menu_label.visible = false


func _on_menu_button_pressed() -> void: # Menu toggle
	settings_window.visible = not settings_window.visible


func move_window(Offset:Vector2i) -> void: # Move Niko
	DisplayServer.window_set_position(DisplayServer.window_get_position(main_window_id) + Vector2i(Offset), main_window_id);



# Settings some funtions
func _on_gaming_mode_toggled(toggled_on: bool) -> void: # Set gaming mode
		GlobalControlls.set_gaming_mode(toggled_on)


func _on_close_button_pressed() -> void: # Close button work
	GlobalControlls.try_quit()


func _on_timer_tick() -> void: # CPS Counter
	@warning_ignore("integer_division")
	cps = timed_clicks / int($Timer.wait_time)
	cps_counter.text = " CPS: " + str(cps)
	timed_clicks = 0
	if not in_anim and not animator.is_playing() and not exit_button_hover:
		if peaceful_mode or cps < scare_threshold:
			rect_for_anim.texture = GlobalControlls.idle_facepick
		else:
			rect_for_anim.texture = GlobalControlls.scare_facepick


func _on_peaceful_mode_toggled(toggled_on: bool) -> void:
	peaceful_mode = toggled_on


func _on_close_button_mouse_entered() -> void:
	rect_for_anim.texture = niko_sad
	exit_button_hover = true


func _on_close_button_mouse_exited() -> void:
	rect_for_anim.texture = GlobalControlls.idle_facepick
	exit_button_hover = false
