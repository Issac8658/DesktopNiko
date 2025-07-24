extends Node

const main_window_id = DisplayServer.MAIN_WINDOW_ID

const anim_duration = .2
const scare_threshold = 6
const afk_time : int = 180
const original_size = Vector2i(124, 152)
const scales = [0.5, 1.0, 2.0, 3.0, 4.0]
const sleepy_afk_time : int = 900

@onready var main_window = get_window()

@export_group("resources")
@export var meow_sounds : Array[AudioStream]

@export_group("objects")
@export var window_to_open : Window # window to open when you click on the menu button
@export var niko_sound : AudioStreamPlayer
@export var animator : AnimationPlayer
@export var niko_layer : CanvasLayer
@export var niko_rect : TextureRect
@export var facepicks_container : Control
@export var meow_sound_option_button : OptionButton
@export var click_count_label : Label
@export var cps_counter : Label
@export var menu_label : Control
@export var sleeping_particles : GPUParticles2D

var is_scared : bool = false
var mouse_offset : Vector2;
var is_dragging : bool = false;
var PositionBeforeMove : Vector2i;
var in_anim : bool = false

var current_sleep_afk_time : int = 0
var save_afk_time : int = 0
var timed_clicks : int = 0
var cps : int = 0


func _ready() -> void: # applying saved parameters
	GlobalControlls.facepick_update.connect(func ():
		update_facepick()
	)
	
	GlobalControlls.niko_scale_changed.connect(update_scale)
	update_scale()
	
	niko_sound.stream = meow_sounds[GlobalControlls.current_meow_sound_id]
	meow_sound_option_button.selected = GlobalControlls.current_meow_sound_id
	click_count_label.text = str(GlobalControlls.clicks)
	update_facepick()
	PassthroughModule.UpdateWindowsExStyles(get_window(), true)

#func _process(_delta: float) -> void: # show again if niko minimized(hidden)
	#if main_window.mode == Window.MODE_MINIMIZED:
	#	main_window.mode = Window.MODE_WINDOWED
	#pass

# Niko functions
func _on_animation_finished(_anim_name: StringName) -> void: #afk animation stop
	animator.stop()
	update_facepick()


func update_scale():
	DisplayServer.window_set_size(original_size * scales[GlobalControlls.niko_scale])
	niko_layer.scale = Vector2(scales[GlobalControlls.niko_scale],scales[GlobalControlls.niko_scale])


func _on_niko_input(event: InputEvent) -> void: # main Niko operations
	save_afk_time = 0
	if animator.is_playing():
		_on_animation_finished("") # cancel afk animation on any input
	if not GlobalControlls.force_facepick:
		if event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_LEFT:
			if event.is_pressed():
				click()
				is_dragging = true
				mouse_offset = event.position
			else:
				is_dragging = false
		if event is InputEventMouseMotion and is_dragging: # dragging
			move_window(event.position - mouse_offset);


func _input(event) -> void: # Niko flip
	if event.is_action_pressed("flip"):
		niko_rect.flip_h = not niko_rect.flip_h


func anim_niko(): # Niko click facepick animation
	in_anim = true
	update_facepick()
	await get_tree().create_timer(anim_duration).timeout
	in_anim = false
	update_facepick()


func click():
	current_sleep_afk_time = 0
	GlobalControlls.clicks += 1
	timed_clicks += 1
	click_count_label.text = str(GlobalControlls.clicks)
	GlobalControlls.clicked.emit()
	niko_sound.play() # meow
	anim_niko() # :O
	
	if not AchievementsGlobalConroller.is_achievement_taked("ten_clicks") and GlobalControlls.clicks >= 10:
		AchievementsGlobalConroller.take_achievement("ten_clicks")
	if not AchievementsGlobalConroller.is_achievement_taked("one_hundred_clicks") and GlobalControlls.clicks >= 100:
		AchievementsGlobalConroller.take_achievement("one_hundred_clicks")
	if not AchievementsGlobalConroller.is_achievement_taked("one_thousand_clicks") and GlobalControlls.clicks >= 1000:
		AchievementsGlobalConroller.take_achievement("one_thousand_clicks")
	if not AchievementsGlobalConroller.is_achievement_taked("one_hundred_thousand_clicks") and GlobalControlls.clicks >= 100000:
		AchievementsGlobalConroller.take_achievement("one_hundred_thousand_clicks")
	if not AchievementsGlobalConroller.is_achievement_taked("one_million_clicks") and GlobalControlls.clicks >= 1000000:
		AchievementsGlobalConroller.take_achievement("one_million_clicks")


func update_facepick():
	sleeping_particles.emitting = false
	if not animator.is_playing():
		if GlobalControlls.force_facepick:
			niko_rect.texture = NikoSpritesModule.get_sprite(GlobalControlls.forced_facepick_id)
		elif GlobalControlls.is_shutdown_popup_shown:
			niko_rect.texture = NikoSpritesModule.get_sprite("niko_cry")
			save_afk_time = 0
			current_sleep_afk_time = 0
		elif GlobalControlls.is_exit_button_hovered:
			niko_rect.texture = NikoSpritesModule.get_sprite("niko_sad")
		elif current_sleep_afk_time >= GlobalControlls.sleep_time + 20:
			sleeping_particles.emitting = true
			niko_rect.texture = NikoSpritesModule.get_sprite("niko_sleep")
			if not AchievementsGlobalConroller.is_achievement_taked("sweet_dreams"):
				AchievementsGlobalConroller.take_achievement("sweet_dreams")
		elif current_sleep_afk_time >= GlobalControlls.sleep_time:
			niko_rect.texture = NikoSpritesModule.get_sprite("niko_sleepy")
		elif in_anim:
			if is_scared:
				niko_rect.texture = NikoSpritesModule.get_sprite(GlobalControlls.scare_speak_facepick)
			else:
				niko_rect.texture = NikoSpritesModule.get_sprite(GlobalControlls.speak_facepick)
		else:
			if is_scared:
				niko_rect.texture = NikoSpritesModule.get_sprite(GlobalControlls.scare_facepick)
			else:
				niko_rect.texture = NikoSpritesModule.get_sprite(GlobalControlls.idle_facepick)

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
	if window_to_open.mode == Window.MODE_MINIMIZED and window_to_open.visible:
		window_to_open.visible = false
		window_to_open.mode = Window.MODE_WINDOWED
		window_to_open.visible = true
	else:
		window_to_open.mode = Window.MODE_WINDOWED
		window_to_open.visible = not window_to_open.visible


func move_window(Offset:Vector2i) -> void: # Move Niko
	DisplayServer.window_set_position(DisplayServer.window_get_position(main_window_id) + Vector2i(Offset), main_window_id);



func _on_timer_tick() -> void: # CPS Counter
	@warning_ignore("integer_division")
	cps = timed_clicks / int($Timer.wait_time)
	cps_counter.text = str(cps)
	timed_clicks = 0
	if GlobalControlls.peaceful_mode or cps < scare_threshold:
		is_scared = false
	else:
		is_scared = true
		if not AchievementsGlobalConroller.is_achievement_taked("im_scared"):
			AchievementsGlobalConroller.take_achievement("im_scared")
		if cps >= 100:
			if not AchievementsGlobalConroller.is_achievement_taked("what_you_do"):
				AchievementsGlobalConroller.take_achievement("what_you_do")
	if GlobalControlls.force_facepick:
		if current_sleep_afk_time < sleepy_afk_time:
			save_afk_time += 1
		current_sleep_afk_time += 1
		update_facepick()
		
		GlobalControlls.total_time += 1
		
		if save_afk_time >= afk_time: # looking around if don't do anything for too long
			save_afk_time = 0
			animator.play("niko_look_around")
			GlobalControlls.save()

func _on_close_button_mouse_entered() -> void: # Niko sad facepick on exit button hover
	GlobalControlls.is_exit_button_hovered = true
	update_facepick()


func _on_close_button_mouse_exited() -> void: # Niko default facepick on exit button unhover
	GlobalControlls.is_exit_button_hovered = false
	update_facepick()
