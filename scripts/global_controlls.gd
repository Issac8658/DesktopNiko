extends Node

signal gaming_mode_changed(state: bool)
signal facepick_update()
signal theme_update()
signal quit_request()
signal saving()
signal saved()
@warning_ignore("unused_signal")

@onready var niko_controller = $/root/TheWorldMachine
var global_color_palette : ColorPalette = load("res://themes/twm_theme/purple_color_palette.tres")
var global_color_palette_id = 0

var scripts_to_save = []

var idle_facepick : Texture2D
var speak_facepick : Texture2D
var scare_facepick : Texture2D
var scare_speak_facepick : Texture2D

var is_mouse_sound_enabled = true
var is_shutdown_popup_shown = false
var is_exit_button_hovered = false
var peaceful_mode = false
var show_saving_icon = true
var is_niko_hovered = false

var config_file = ConfigFile.new()
var clicks = 0
var current_meow_sound_id = 0
var language = 0

func _ready() -> void: # loading all parameters
	config_file.load("user://NikoMemories.cfg")
	clicks = config_file.get_value("Main", "Clicks", 0)
	current_meow_sound_id = config_file.get_value("Main", "MeowSoundId", 0)
	
	language = config_file.get_value("Settings", "Language", 0)
	niko_controller.facepicks_container.get_node("Idle/OptionButton"
	).selected = config_file.get_value("Settings", "IdleFacepick", 0)
	niko_controller.facepicks_container.get_node("Speak/OptionButton"
	).selected = config_file.get_value("Settings", "SpeakFacepick", 1)
	niko_controller.facepicks_container.get_node("Scare/OptionButton"
	).selected = config_file.get_value("Settings", "ScareFacepick", 2)
	niko_controller.facepicks_container.get_node("ScareSpeak/OptionButton"
	).selected = config_file.get_value("Settings", "ScareSpeakFacepick", 3)
	
	global_color_palette_id = config_file.get_value("Settings", "Theme", 0)
	is_mouse_sound_enabled = config_file.get_value("Settings", "ClickSound", true)
	show_saving_icon = config_file.get_value("Settings", "SavingIcon", true)
	
	DisplayServer.window_set_position(config_file.get_value("Main", "NikoPosition", get_default_pos()), DisplayServer.MAIN_WINDOW_ID)
	
	facepick_update.emit()
	
	for child in $/root/TheWorldMachine.get_children():
		if child is Window:
			match child.visible:
				false: child.process_mode = Node.PROCESS_MODE_DISABLED
				true: child.process_mode = Node.PROCESS_MODE_INHERIT
			child.visibility_changed.connect(func ():
				match child.visible:
					false: child.process_mode = Node.PROCESS_MODE_DISABLED
					true: child.process_mode = Node.PROCESS_MODE_INHERIT
			)
	print("Hello, Niko!")


func _notification(what): # exit request capture
	if what == NOTIFICATION_WM_CLOSE_REQUEST:
		quit_request.emit()


func save():
	saving.emit()
	
	config_file.set_value("Main", "Clicks", clicks)
	config_file.set_value("Main", "MeowSoundId", current_meow_sound_id)
	
	config_file.set_value("Settings", "Language", language)
	config_file.set_value("Settings", "IdleFacepick",
	niko_controller.facepicks_container.get_node("Idle/OptionButton"
	).selected)
	config_file.set_value("Settings", "SpeakFacepick",
	niko_controller.facepicks_container.get_node("Speak/OptionButton"
	).selected)
	config_file.set_value("Settings", "ScareFacepick",
	niko_controller.facepicks_container.get_node("Scare/OptionButton"
	).selected)
	config_file.set_value("Settings", "ScareSpeakFacepick",
	niko_controller.facepicks_container.get_node("ScareSpeak/OptionButton"
	).selected)
	
	config_file.set_value("Settings", "Theme", global_color_palette_id)
	config_file.set_value("Settings", "ClickSound", is_mouse_sound_enabled)
	config_file.set_value("Settings", "SavingIcon", show_saving_icon)
	
	config_file.set_value("Main", "NikoPosition",
	DisplayServer.window_get_position(DisplayServer.MAIN_WINDOW_ID))
	
	config_file.save("user://NikoMemories.cfg")
	
	for script in scripts_to_save:
		script.save()
	
	saved.emit()


func try_quit(): # saving all parameters and exit
	save()
	print("Goodbye, Niko!")
	get_tree().quit()


func set_gaming_mode(state: bool): # setting gaming mode window state
	DisplayServer.window_set_flag(DisplayServer.WINDOW_FLAG_NO_FOCUS, state, DisplayServer.MAIN_WINDOW_ID)
	DisplayServer.window_set_flag(DisplayServer.WINDOW_FLAG_MOUSE_PASSTHROUGH, state, DisplayServer.MAIN_WINDOW_ID)
	gaming_mode_changed.emit(state)


func set_theme(color_palette : ColorPalette, theme_id : int):
	global_color_palette = color_palette
	global_color_palette_id = theme_id
	theme_update.emit()


func get_default_pos(): # calculatig center of main screen
	var screen_pos = DisplayServer.screen_get_position(DisplayServer.get_primary_screen())
	var result_pos = screen_pos + DisplayServer.screen_get_size(DisplayServer.get_primary_screen())/2
	return result_pos
