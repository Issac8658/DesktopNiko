extends Node

@onready var gaming_mod_checkbox = get_node("GamingMode")
const locales = ["en-US", "ru-RU"]

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	gaming_mod_checkbox.toggled.connect(func(toggled):
		GlobalControlls.set_gaming_mode(toggled)
	)
	
	get_node("Peaceful").toggled.connect(func(toggled):
		GlobalControlls.peaceful_mode = toggled
		GlobalControlls.facepick_update.emit()
	)
	
	GlobalControlls.gaming_mode_changed.connect(func (state):
		gaming_mod_checkbox.button_pressed = state
	)
	
	var click_sound_checkbox = get_node("ClickSound")
	click_sound_checkbox.button_pressed = GlobalControlls.is_mouse_sound_enabled
	click_sound_checkbox.toggled.connect(func (toggled):
		GlobalControlls.is_mouse_sound_enabled = toggled
	)
	
	var saving_icon_checkbox = get_node("SavingIcon")
	saving_icon_checkbox.button_pressed = GlobalControlls.show_saving_icon
	saving_icon_checkbox.toggled.connect(func (toggled):
		GlobalControlls.show_saving_icon = toggled
	)
	
	var language_option_button = get_node("Language/LanguageSelectionOptionButton")
	TranslationServer.set_locale(locales[GlobalControlls.language])
	language_option_button.selected = GlobalControlls.language
	language_option_button.item_selected.connect(func (id):
		GlobalControlls.language = id
		TranslationServer.set_locale(locales[id])
	)
