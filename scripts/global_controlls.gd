extends Node

signal gaming_mode_changed(state: bool)
signal facepick_update()
signal theme_update()
signal quit_request()
signal saving()
signal saved()
@warning_ignore("unused_signal")
signal clicked()
@warning_ignore("unused_signal")
signal niko_scale_changed()

@onready var niko_controller = $/root/TheWorldMachine
var global_color_palette : ColorPalette = load("res://themes/twm_theme/purple_color_palette.tres")
var global_color_palette_id = 0

var scripts_to_save = []

var idle_facepick : String = "niko_smile"
var speak_facepick : String = "niko_speak"
var scare_facepick : String = "niko_shock"
var scare_speak_facepick : String = "niko_surprised"

var is_mouse_sound_enabled = true
var is_shutdown_popup_shown = false
var is_exit_button_hovered = false
var peaceful_mode = false
var show_saving_icon = true
var is_niko_hovered = false

var config_file = ConfigFile.new()
var clicks : int = 0
var current_meow_sound_id : int = 0
var language : int = 0
var total_time : int = 0
var niko_scale : int = 1
var sleep_time : int = 900
var use_legacy_sprites : bool = false
var show_achievements : bool = true

func _ready() -> void: # loading all parameters
	config_file.load("user://NikoMemories.cfg")
	clicks = config_file.get_value("Main", "Clicks", 0)
	current_meow_sound_id = config_file.get_value("Main", "MeowSoundId", 0)
	total_time = config_file.get_value("Main", "TotalTime", 0)
	niko_scale = config_file.get_value("Main", "NikoScale", 1)
	AchievementsGlobalConroller.taked_achievements = config_file.get_value("Main", "Achievements", [])
	if AchievementsGlobalConroller.is_achievement_taked("sweet_dreams"):
		sleep_time = config_file.get_value("Main", "SleepTime", 900)
	
	language = config_file.get_value("Settings", "Language", 0)
	idle_facepick = str(config_file.get_value("Settings", "IdleFacepick", "niko_smile"))
	speak_facepick = str(config_file.get_value("Settings", "SpeakFacepick", "niko_speak"))
	scare_facepick = str(config_file.get_value("Settings", "ScareFacepick", "niko_shock"))
	scare_speak_facepick = str(config_file.get_value("Settings", "ScareSpeakFacepick", "niko_surprised"))
	
	global_color_palette_id = config_file.get_value("Settings", "Theme", 0)
	is_mouse_sound_enabled = config_file.get_value("Settings", "ClickSound", true)
	show_saving_icon = config_file.get_value("Settings", "SavingIcon", true)
	AudioServer.set_bus_volume_linear(0, config_file.get_value("Settings", "MasterVolume", 1))
	AudioServer.set_bus_volume_linear(1, config_file.get_value("Settings", "MeowVolume", 1))
	use_legacy_sprites = config_file.get_value("Settings", "LegacySprites", false)
	show_achievements = config_file.get_value("Settings", "ShowAchievements", true)
	
	DisplayServer.window_set_position(config_file.get_value("Main", "NikoPosition", get_default_pos()), DisplayServer.MAIN_WINDOW_ID)
	
	facepick_update.emit()
	
	for child in $/root/TheWorldMachine.get_children():
		if child is Window:
			if not child.process_mode == Node.PROCESS_MODE_ALWAYS:
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
	
	config_file.set_value("Main", "SaveVersion", "1.1.3")
	config_file.set_value("Main", "Clicks", clicks)
	config_file.set_value("Main", "MeowSoundId", current_meow_sound_id)
	config_file.set_value("Main", "TotalTime", total_time)
	config_file.set_value("Main", "NikoScale", niko_scale)
	config_file.set_value("Main", "Achievements", AchievementsGlobalConroller.taked_achievements)
	if AchievementsGlobalConroller.is_achievement_taked("sweet_dreams"):
		config_file.set_value("Main", "SleepTime", sleep_time)
	
	config_file.set_value("Settings", "Language", language)
	config_file.set_value("Settings", "IdleFacepick", idle_facepick)
	config_file.set_value("Settings", "SpeakFacepick", speak_facepick)
	config_file.set_value("Settings", "ScareFacepick", scare_facepick)
	config_file.set_value("Settings", "ScareSpeakFacepick", scare_speak_facepick)
	
	config_file.set_value("Settings", "Theme", global_color_palette_id)
	config_file.set_value("Settings", "ClickSound", is_mouse_sound_enabled)
	config_file.set_value("Settings", "SavingIcon", show_saving_icon)
	config_file.set_value("Settings", "MasterVolume", AudioServer.get_bus_volume_linear(0))
	config_file.set_value("Settings", "MeowVolume", AudioServer.get_bus_volume_linear(1))
	config_file.set_value("Settings", "LegacySprites", use_legacy_sprites)
	config_file.set_value("Settings", "ShowAchievements", show_achievements)
	
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
