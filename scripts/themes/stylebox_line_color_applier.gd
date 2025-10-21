extends StyleBoxLine

enum PaletteColor {default, icon_hover_tint, hover_outline, hover_base, focus_outline, focus_base, button_hover, default_bg_color}

@export var _base_color : PaletteColor = PaletteColor.default:
	set(value):
		_base_color = value
		update_colors()

func _init(s_base_color = PaletteColor.default) -> void:
	_base_color = s_base_color
	update_colors()
	#GlobalControlls.theme_update.connect(func():
	#	update_colors()
	#)

func update_colors():
	#color = GlobalControlls.global_color_palette.colors[_base_color]
	pass
