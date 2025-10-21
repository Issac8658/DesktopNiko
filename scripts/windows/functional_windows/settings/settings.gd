extends Node

const locales = ["en-US", "ru-RU", "de-DE", "ua-UA"]

@export_group("General")
@export var gaming_mod_checkbox : CheckBox
@export var peaceful_check_box : CheckBox
@export var click_sound_checkbox : CheckBox
@export var saving_icon_checkbox : CheckBox
@export var show_achievements_checkbox : CheckBox
@export var language_option_button : OptionButton
@export_group("Niko")
@export var legacy_facepicks_checkbox : CheckBox
@export var niko_scale_option_button : OptionButton
@export var niko_facepicks_select : Node
@export var snap_to_bottom_button : Button
@export_group("Other")
@export var reset_all_data_button : Button
@export var do_events_checkbox : CheckBox
@export var niko_always_on_top_checkbox : CheckBox
@export var windows_always_on_top_checkbox : CheckBox
@export var taskbar_icon_checkbox : CheckBox
@export var data_reset_popup : Window
@export var save_button : Button

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	gaming_mod_checkbox.toggled.connect(func(toggled):
		ValuesContainer.GamingModeEnabled = toggled
	)
	
	peaceful_check_box.toggled.connect(func(toggled):
		ValuesContainer.PeacfulMode = toggled
		#GlobalControlls.facepick_update.emit()
	)
	
	ValuesContainer.GamingModeToggled.connect(func (Toggled):
		gaming_mod_checkbox.button_pressed = Toggled
	)
	
	click_sound_checkbox.button_pressed = ValuesContainer.MouseSoundEnabled
	click_sound_checkbox.toggled.connect(func (toggled):
		ValuesContainer.MouseSoundEnabled = toggled
	)
	
	saving_icon_checkbox.button_pressed = ValuesContainer.ShowSavingIcon
	saving_icon_checkbox.toggled.connect(func (toggled):
		ValuesContainer.ShowSavingIcon = toggled
	)
	
	show_achievements_checkbox.button_pressed = ValuesContainer.ShowAchievements
	show_achievements_checkbox.toggled.connect(func (toggled):
		ValuesContainer.ShowAchievements = toggled
	)
	
	TranslationServer.set_locale(locales[ValuesContainer.Language])
	language_option_button.selected = ValuesContainer.Language
	language_option_button.item_selected.connect(func (id):
		ValuesContainer.Language = id
		TranslationServer.set_locale(locales[id])
	)
	
	niko_scale_option_button.selected = ValuesContainer.NikoScale
	niko_scale_option_button.item_selected.connect(func (id):
		ValuesContainer.NikoScale = id
	)
	
	#legacy_facepicks_checkbox.toggled.connect(func (toggled):
	#	GlobalControlls.use_legacy_sprites = toggled
	#	niko_facepicks_select.update_facepicks_preview()
	#)
	
	do_events_checkbox.toggled.connect(func (toggled):
		ValuesContainer.DoEvents = toggled
	)
	
	niko_always_on_top_checkbox.toggled.connect(func (toggled):
		ValuesContainer.NikoAlwaysOnTop = toggled
	)
	windows_always_on_top_checkbox.toggled.connect(func (toggled):
		ValuesContainer.WindowsAlwaysOnTop = toggled
	)
	taskbar_icon_checkbox.toggled.connect(func (toggled):
		ValuesContainer.ShowTaskbarIcon = toggled
	)
	
	reset_all_data_button.pressed.connect(func ():
		data_reset_popup.popup()
	)
	data_reset_popup.yes_button_pressed.connect(func ():
		print(DirAccess.remove_absolute(ProjectSettings.globalize_path("user://")))
		get_tree().quit()
	)
	save_button.pressed.connect(func ():
		GlobalController.Save()
	)
	snap_to_bottom_button.toggled.connect(func (toggled):
		ValuesContainer.SnapToBottom = toggled
	)
