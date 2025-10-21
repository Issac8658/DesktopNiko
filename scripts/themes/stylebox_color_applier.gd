extends StyleBoxFlat

enum PaletteColor {default, button_hover, hover_outline, hover_base, focus_outline, focus_base, default_bg_color}

@export var _base_color : PaletteColor = PaletteColor.default:
	set(value):
		_base_color = value
		update_colors()
@export var _border_color : PaletteColor = PaletteColor.default:
	set(value):
		_border_color = value
		update_colors()

func _init(s_base_color = PaletteColor.default, s_border_color = PaletteColor.default) -> void:
	_base_color = s_base_color
	_border_color = s_border_color
	update_colors()
	#GlobalControlls.theme_update.connect(func():
	#	update_colors()
	#)

func update_colors():
	#bg_color = GlobalControlls.global_color_palette.colors[_base_color]
	#border_color = GlobalControlls.global_color_palette.colors[_border_color]
	pass
