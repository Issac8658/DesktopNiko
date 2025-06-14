extends Node

signal gaming_mode_changed(state: bool)
signal idle_facepick_update()

@onready var niko_controller = $/root/TheWorldMachine

var idle_facepick : Texture2D
var speak_facepick : Texture2D
var scare_facepick : Texture2D
var scare_speak_facepick : Texture2D

var config_file = ConfigFile.new()
var clicks = 0
var current_meow_sound_id = 0

func _ready() -> void:
	config_file.load("user://NikoMemories.cfg")
	clicks = config_file.get_value("Main", "Clicks", 0)
	current_meow_sound_id = config_file.get_value("Main", "MeowSoundId", 0)
	
	niko_controller.facepicks_container.get_node("Idle/OptionButton"
	).selected = config_file.get_value("Settings", "IdleFacepick", 0)
	niko_controller.facepicks_container.get_node("Speak/OptionButton"
	).selected = config_file.get_value("Settings", "SpeakFacepick", 1)
	niko_controller.facepicks_container.get_node("Scare/OptionButton"
	).selected = config_file.get_value("Settings", "ScareFacepick", 2)
	niko_controller.facepicks_container.get_node("ScareSpeak/OptionButton"
	).selected = config_file.get_value("Settings", "ScareSpea/kFacepick", 3)
	
	DisplayServer.window_set_position(config_file.get_value("Main", "NikoPosition", get_default_pos()), DisplayServer.MAIN_WINDOW_ID)
	
	idle_facepick_update.emit()


func _notification(what):
	if what == NOTIFICATION_WM_CLOSE_REQUEST:
		try_quit()


func get_default_pos():
	var screen_pos = DisplayServer.screen_get_position(DisplayServer.get_primary_screen())
	var result_pos = screen_pos + DisplayServer.screen_get_size(DisplayServer.get_primary_screen())/2
	return result_pos


func try_quit():
		config_file.set_value("Main", "Clicks", clicks)
		config_file.set_value("Main", "MeowSoundId", current_meow_sound_id)
		
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
		
		config_file.set_value("Main", "NikoPosition", DisplayServer.window_get_position(DisplayServer.MAIN_WINDOW_ID))
		
		config_file.save("user://NikoMemories.cfg")
		get_tree().quit()


func set_gaming_mode(state: bool):
	DisplayServer.window_set_flag(DisplayServer.WINDOW_FLAG_NO_FOCUS, state, DisplayServer.MAIN_WINDOW_ID)
	DisplayServer.window_set_flag(DisplayServer.WINDOW_FLAG_MOUSE_PASSTHROUGH, state, DisplayServer.MAIN_WINDOW_ID)
	gaming_mode_changed.emit(state)
