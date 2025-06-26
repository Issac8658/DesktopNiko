extends Control

@export var Palettes : Array[ColorPalette]

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	var id = 0
	for theme_button in get_children():
		theme_button.get_node("Button").gui_input.connect(func (event):
			if event is InputEventMouseButton and event.double_click and not GlobalControlls.global_color_palette == Palettes[id]:
				GlobalControlls.set_theme(Palettes[id], id)
		)
		id += 1
	
	var theme_id = GlobalControlls.global_color_palette_id
	GlobalControlls.set_theme(Palettes[theme_id], theme_id)
