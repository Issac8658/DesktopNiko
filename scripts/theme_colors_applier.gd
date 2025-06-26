extends Theme

func _init() -> void:
	update_colors()
	GlobalControlls.theme_update.connect(func():
		update_colors()
	)
	
func update_colors():
	# Label
	set_color("font_color", "Label", GlobalControlls.global_color_palette.colors[0])
	# Button
	set_color("font_color", "Button", GlobalControlls.global_color_palette.colors[0])
	set_color("font_focus_color", "Button", GlobalControlls.global_color_palette.colors[0])
	set_color("font_hover_color", "Button", GlobalControlls.global_color_palette.colors[1])
	set_color("font_hover_pressed_color", "Button", GlobalControlls.global_color_palette.colors[0])
	set_color("font_pressed_color", "Button", GlobalControlls.global_color_palette.colors[0])
	set_color("icon_disabled_color", "Button", GlobalControlls.global_color_palette.colors[0])
	set_color("icon_hover_color", "Button", GlobalControlls.global_color_palette.colors[1])
	set_color("icon_normal_color", "Button", GlobalControlls.global_color_palette.colors[0])
	# Icon Button
	set_color("icon_disabled_color", "IconButton", GlobalControlls.global_color_palette.colors[0])
	set_color("icon_focus_color", "IconButton", GlobalControlls.global_color_palette.colors[0])
	set_color("icon_hover_color", "IconButton", GlobalControlls.global_color_palette.colors[1])
	set_color("icon_hover_pressed_color", "IconButton", GlobalControlls.global_color_palette.colors[1])
	set_color("icon_normal_color", "IconButton", GlobalControlls.global_color_palette.colors[0])
	set_color("icon_pressed_color", "IconButton", GlobalControlls.global_color_palette.colors[1])
	# PopupMenu
	set_color("font_color", "PopupMenu", GlobalControlls.global_color_palette.colors[0])
	set_color("font_hover_color", "PopupMenu", GlobalControlls.global_color_palette.colors[1])
	set_color("font_separator_color", "PopupMenu", GlobalControlls.global_color_palette.colors[0])
	#Other
	set_color("icon_normal_color", "AlwaysActivatedButton", GlobalControlls.global_color_palette.colors[1])
