# t.me/Kobachek228 ты заебал

extends Node
# some signals
signal gaming_mode_changed()
signal facepick_update()
signal theme_update()
signal quit_request()
signal saving()
signal saved()
@warning_ignore("unused_signal")
signal clicked()
@warning_ignore("unused_signal")
signal niko_scale_changed()
@warning_ignore("unused_signal")
signal event_started(event_name : String)
signal niko_visibility_changed(visible : bool)
#@warning_ignore("unused_signal")
#signal event_ended(event_name : String)
# niko controller, idk
@onready var niko_controller = $/root/TheWorldMachine
# some themes shit
var global_color_palette : ColorPalette = load("res://themes/twm_theme/purple_color_palette.tres")
var global_color_palette_id = 0
# scripts list with .save() func
var scripts_to_save = []
# current facepicks
var idle_facepick : String = "niko_smile"
var speak_facepick : String = "niko_speak"
var scare_facepick : String = "niko_shock"
var scare_speak_facepick : String = "niko_surprised"
# some system vars
var is_shutdown_popup_shown : bool = false
var is_exit_button_hovered : bool = false
var peaceful_mode : bool = false
var is_niko_hovered : bool = false
var gaming_mode_enabled : bool = false
var force_facepick : bool = false
var forced_facepick_id : String

var config_file = ConfigFile.new()
# vars to save 
var clicks : int = 0
var current_meow_sound_id : int = 0
var language : int = 0 # 0 - eng, 1 - rus, 2 - deu, 3 - ukr
var total_time : int = 0 # all work time in seconds
var niko_scale : int = 1 # 0 - 0.5x, 1 - 1x, 2 - 2x, 3 - 3x, 4 - 4x
var sleep_time : int = 900 # in seconds
var snap_to_bottom : bool = true
var doned_events : int = 0 # events that the player has successfully completed
var is_mouse_sound_enabled : bool = true
var show_saving_icon : bool = true
var use_legacy_sprites : bool = false
var show_achievements : bool = true
var do_events : bool = true
var niko_always_on_top : bool = true:
	set(value):
		niko_always_on_top = value
		get_window().always_on_top = value
		PassthroughModule.UpdateWindowsExStyles(get_window(), not taskbar_icon, true)
var windows_always_on_top : bool = false:
	set(value):
		windows_always_on_top = value
		for child in $/root/TheWorldMachine.get_children():
			if child is Window:
				if not child.transient:
					child.always_on_top = value
					if child.visible:
						child.visible = not child.visible
						child.visible = not child.visible
var taskbar_icon : bool = true:
	set(value):
		taskbar_icon = value
		PassthroughModule.UpdateWindowsExStyles(get_window(), not taskbar_icon, true)

func _ready() -> void: # loading all parameters
	config_file.load("user://NikoMemories.cfg")
	clicks = config_file.get_value("Main", "Clicks", 0)
	current_meow_sound_id = config_file.get_value("Main", "MeowSoundId", 0)
	total_time = config_file.get_value("Main", "TotalTime", 0)
	niko_scale = config_file.get_value("Main", "NikoScale", 1)
	AchievementsGlobalConroller.taked_achievements = config_file.get_value("Main", "Achievements", [])
	if AchievementsGlobalConroller.is_achievement_taked("sweet_dreams"):
		sleep_time = config_file.get_value("Main", "SleepTime", 900)
	doned_events = config_file.get_value("Main", "DonedEvents", 0)
	
	language = config_file.get_value("Settings", "Language", 0)
	if not config_file.get_value("Settings", "IdleFacepick", "niko_smile") is int:
		idle_facepick = config_file.get_value("Settings", "IdleFacepick", "niko_smile")
		speak_facepick = config_file.get_value("Settings", "SpeakFacepick", "niko_speak")
		scare_facepick = config_file.get_value("Settings", "ScareFacepick", "niko_shock")
		scare_speak_facepick = config_file.get_value("Settings", "ScareSpeakFacepick", "niko_surprised")
	
	global_color_palette_id = config_file.get_value("Settings", "Theme", 0)
	is_mouse_sound_enabled = config_file.get_value("Settings", "ClickSound", true)
	show_saving_icon = config_file.get_value("Settings", "SavingIcon", true)
	AudioServer.set_bus_volume_linear(0, config_file.get_value("Settings", "MasterVolume", 1))
	AudioServer.set_bus_volume_linear(1, config_file.get_value("Settings", "MeowVolume", 1))
	use_legacy_sprites = config_file.get_value("Settings", "LegacySprites", false)
	show_achievements = config_file.get_value("Settings", "ShowAchievements", true)
	doned_events = config_file.get_value("Settings", "DoEvents", true)
	niko_always_on_top = config_file.get_value("Settings", "NikoAlwaysOnTop", true)
	windows_always_on_top = config_file.get_value("Settings", "WindowsAlwaysOnTop", true)
	taskbar_icon = config_file.get_value("Settings", "TaskbarIcon", true)
	
	DisplayServer.window_set_position(config_file.get_value("Main", "NikoPosition", get_default_pos()), DisplayServer.MAIN_WINDOW_ID)
	
	facepick_update.emit()
	# freezing hidden windows
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
	print("Hello, Niko!") # Hello!! :З


func _notification(what): # exit request capture
	if what == NOTIFICATION_WM_CLOSE_REQUEST:
		quit_request.emit()


func save():
	saving.emit()
	
	config_file.set_value("Main", "SaveVersion", ProjectSettings.get_setting("application/config/version"))
	config_file.set_value("Main", "Clicks", clicks)
	config_file.set_value("Main", "MeowSoundId", current_meow_sound_id)
	config_file.set_value("Main", "TotalTime", total_time)
	config_file.set_value("Main", "NikoScale", niko_scale)
	config_file.set_value("Main", "Achievements", AchievementsGlobalConroller.taked_achievements)
	if AchievementsGlobalConroller.is_achievement_taked("sweet_dreams"):
		config_file.set_value("Main", "SleepTime", sleep_time)
	config_file.set_value("Main", "DonedEvents", doned_events)
	
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
	config_file.set_value("Settings", "DoEvents", do_events)
	config_file.set_value("Settings", "NikoAlwaysOnTop", niko_always_on_top)
	config_file.set_value("Settings", "WindowsAlwaysOnTop", windows_always_on_top)
	config_file.set_value("Settings", "TaskbarIcon", taskbar_icon)
	
	config_file.set_value("Main", "NikoPosition",
	DisplayServer.window_get_position(DisplayServer.MAIN_WINDOW_ID))
	
	config_file.save("user://NikoMemories.cfg")
	
	for script : Node in scripts_to_save:
		if script.has_method("save"):
			script.save()
	
	saved.emit()


func try_quit(): # saving all parameters and exit
	save()
	print("Goodbye, Niko!")
	get_tree().quit()


func set_gaming_mode(state: bool): # setting gaming mode window state
	DisplayServer.window_set_flag(DisplayServer.WINDOW_FLAG_NO_FOCUS, state, DisplayServer.MAIN_WINDOW_ID)
	DisplayServer.window_set_flag(DisplayServer.WINDOW_FLAG_MOUSE_PASSTHROUGH, state, DisplayServer.MAIN_WINDOW_ID)
	PassthroughModule.UpdateWindowsExStyles(get_window(), not taskbar_icon)
	gaming_mode_enabled = state
	gaming_mode_changed.emit()


func set_theme(color_palette : ColorPalette, theme_id : int):
	global_color_palette = color_palette
	global_color_palette_id = theme_id
	theme_update.emit()

func set_forced_facepick(facepick_id : String):
	if NikoSpritesModule.has_sprite(facepick_id):
		forced_facepick_id = facepick_id
		force_facepick = true
		facepick_update.emit()

func set_niko_visibility(visible : bool):
	niko_visibility_changed.emit(visible)

func unforce_facepick():
	force_facepick = false
	facepick_update.emit()

func get_default_pos(): # calculatig center of main screen
	var screen_pos = DisplayServer.screen_get_position(DisplayServer.get_primary_screen())
	var result_pos = screen_pos + DisplayServer.screen_get_size(DisplayServer.get_primary_screen())/2
	return result_pos
