extends Control
class_name SellerGraphicDrawer

func _ready() -> void:
	GlobalControlls.second_ticked.connect(func ():
		queue_redraw()
	)

func _draw() -> void:
	draw_line(Vector2.ZERO, Vector2(size.x, PancakeClickerGlobalController.pancakes), GlobalControlls.global_color_palette.colors[0], 2)
