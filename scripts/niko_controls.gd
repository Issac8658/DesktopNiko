extends Node

const main_window_id = DisplayServer.MAIN_WINDOW_ID

const anim_duration = .2
const scare_threshold = 6
const afk_time : int = 180

@onready var main_window = get_window()

@export_group("resources")
@export var niko_sad : Texture2D
@export var niko_cry : Texture2D
@export var meow_sounds : Array[AudioStream]

@export_group("objects")
@export var settings_window : Window
@export var niko_sound : AudioStreamPlayer
@export var animator : AnimationPlayer
@export var niko_rect : TextureRect
@export var facepicks_container : Control
@export var meow_sound_option_button : OptionButton
@export var click_count_label : Label
@export var cps_counter : Label
@export var menu_label : Control

var is_scared = false
var mouse_offset : Vector2;
var is_dragging : bool = false;
var PositionBeforeMove : Vector2i;
var in_anim = false

var current_afk_time : int = 0
var timed_clicks : int = 0
var cps : int = 0
#var time = 0


func _ready() -> void: # applying saved parameters
	GlobalControlls.facepick_update.connect(func ():
		update_facepick()
	)
	niko_sound.stream = meow_sounds[GlobalControlls.current_meow_sound_id]
	meow_sound_option_button.selected = GlobalControlls.current_meow_sound_id
	click_count_label.text = str(GlobalControlls.clicks)
	update_facepick()

func _process(_delta: float) -> void:
	if main_window.mode == Window.MODE_MINIMIZED:
		main_window.mode = Window.MODE_WINDOWED


# Niko functions
func _on_animation_finished(_anim_name: StringName) -> void: #afk animation stop
	animator.stop()
	update_facepick()


func _on_niko_input(event: InputEvent) -> void: # main Niko operations
	current_afk_time = 0
	if animator.is_playing():
		_on_animation_finished("") # cancel afk animation on any input
		
	if event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_LEFT:
		if event.is_pressed():
			
			niko_sound.play() # meow
			GlobalControlls.clicks += 1
			timed_clicks += 1
			click_count_label.text = str(GlobalControlls.clicks)
			
			anim_niko() # :O
			
			is_dragging = true
			mouse_offset = event.position
		else:
			is_dragging = false
	if event is InputEventMouseMotion and is_dragging: # dragging
		move_window(event.position - mouse_offset);


func _input(event) -> void: # Niko flip
	if event.is_action_pressed("flip"):
		niko_rect.flip_h = not niko_rect.flip_h


func anim_niko(): # Niko facepick animation
	in_anim = true
	update_facepick()
	await get_tree().create_timer(anim_duration).timeout
	in_anim = false
	update_facepick()


func update_facepick():
	if not animator.is_playing():
		if GlobalControlls.is_shutdown_popup_shown:
			niko_rect.texture = niko_cry
			current_afk_time = 0
		elif GlobalControlls.is_exit_button_hovered:
			niko_rect.texture = niko_sad
		elif in_anim:
			if is_scared:
				niko_rect.texture = GlobalControlls.scare_speak_facepick
			else:
				niko_rect.texture = GlobalControlls.speak_facepick
		else:
			if is_scared:
				niko_rect.texture = GlobalControlls.scare_facepick
			else:
				niko_rect.texture = GlobalControlls.idle_facepick


func _on_meow_sound_selected(index: int) -> void: # meow sound changing
	GlobalControlls.current_meow_sound_id = index
	niko_sound.stream = meow_sounds[index]


# Window functions
func _on_window_mouse_entered() -> void: # Niko menu show
	menu_label.visible = true
	GlobalControlls.is_niko_hovered = true


func _on_window_mouse_exited() -> void: # Niko menu hide
	menu_label.visible = false
	GlobalControlls.is_niko_hovered = false


func _on_menu_button_pressed() -> void: # Menu toggle
	settings_window.visible = not settings_window.visible


func move_window(Offset:Vector2i) -> void: # Move Niko
	DisplayServer.window_set_position(DisplayServer.window_get_position(main_window_id) + Vector2i(Offset), main_window_id);



func _on_timer_tick() -> void: # CPS Counter
	@warning_ignore("integer_division")
	cps = timed_clicks / int($Timer.wait_time)
	cps_counter.text = " CPS: " + str(cps)
	timed_clicks = 0
	if GlobalControlls.peaceful_mode or cps < scare_threshold:
		is_scared = false
	else:
		is_scared = true
	current_afk_time += 1
	update_facepick()
	
	if current_afk_time >= afk_time: # looking around if don't do anything for too long
		current_afk_time = 0
		animator.play("niko_look_around")
		GlobalControlls.save()


func _on_close_button_mouse_entered() -> void: # Niko sad facepick on exit button hover
	GlobalControlls.is_exit_button_hovered = true
	update_facepick()


func _on_close_button_mouse_exited() -> void: # Niko default facepick on exit button unhover
	GlobalControlls.is_exit_button_hovered = false
	update_facepick()
