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
		GlobalControlls.set_gaming_mode(toggled)
	)
	
	peaceful_check_box.toggled.connect(func(toggled):
		GlobalControlls.peaceful_mode = toggled
		GlobalControlls.facepick_update.emit()
	)
	
	GlobalControlls.gaming_mode_changed.connect(func ():
		gaming_mod_checkbox.button_pressed = GlobalControlls.gaming_mode_enabled
	)
	
	click_sound_checkbox.button_pressed = GlobalControlls.is_mouse_sound_enabled
	click_sound_checkbox.toggled.connect(func (toggled):
		GlobalControlls.is_mouse_sound_enabled = toggled
	)
	
	saving_icon_checkbox.button_pressed = GlobalControlls.show_saving_icon
	saving_icon_checkbox.toggled.connect(func (toggled):
		GlobalControlls.show_saving_icon = toggled
	)
	
	show_achievements_checkbox.button_pressed = GlobalControlls.show_achievements
	show_achievements_checkbox.toggled.connect(func (toggled):
		GlobalControlls.show_achievements = toggled
	)
	
	TranslationServer.set_locale(locales[GlobalControlls.language])
	language_option_button.selected = GlobalControlls.language
	language_option_button.item_selected.connect(func (id):
		GlobalControlls.language = id
		TranslationServer.set_locale(locales[id])
	)
	
	niko_scale_option_button.selected = GlobalControlls.niko_scale
	niko_scale_option_button.item_selected.connect(func (id):
		GlobalControlls.niko_scale = id
		GlobalControlls.niko_scale_changed.emit()
	)
	
	legacy_facepicks_checkbox.toggled.connect(func (toggled):
		GlobalControlls.use_legacy_sprites = toggled
		niko_facepicks_select.update_facepicks_preview()
	)
	
	do_events_checkbox.toggled.connect(func (toggled):
		GlobalControlls.do_events = toggled
	)
	
	niko_always_on_top_checkbox.toggled.connect(func (toggled):
		GlobalControlls.niko_always_on_top = toggled
	)
	windows_always_on_top_checkbox.toggled.connect(func (toggled):
		GlobalControlls.windows_always_on_top = toggled
	)
	taskbar_icon_checkbox.toggled.connect(func (toggled):
		GlobalControlls.taskbar_icon = toggled
	)
	
	reset_all_data_button.pressed.connect(func ():
		data_reset_popup.popup()
	)
	data_reset_popup.yes_button_pressed.connect(func ():
		print(DirAccess.remove_absolute(ProjectSettings.globalize_path("user://")))
		get_tree().quit()
	)
	save_button.pressed.connect(func ():
		GlobalControlls.save()
	)
	snap_to_bottom_button.toggled.connect(func (toggled):
		GlobalControlls.snap_to_bottom = toggled
	)
