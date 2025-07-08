@icon("res://ui_sprites/shortcuts.png")
extends Control
class_name SellerGraphicDrawer

@export var custom_noise : Noise
@export var label_min : Label
@export var label_max : Label
@export var label_current : Label

@export var save_to : int = 1

@export var max_cost : float = 700

var values : Array = []
const res : float = 50;

var pos : float = 0

func _ready() -> void:
	custom_noise.set("seed", randi_range(0, 10000000))
	
	for i in range(res):
		values.append(0.0)
	
	label_max.visible_characters = 5
	label_min.visible_characters = 5

func _process(delta: float) -> void:
	if is_visible_in_tree():
		for i in range(res):
			var value = (custom_noise.get_noise_1d(pos - i/res*100)/2 + 0.5)
			values[i] = value * (max_cost + PancakeClickerGlobalController.yelli_add) * PancakeClickerGlobalController.yelli_multiplier
		
		pos += delta * 75
		
		match save_to:
			1: PancakeClickerGlobalController.seller_cost_1 = values[0]
			2: PancakeClickerGlobalController.seller_cost_2 = values[0]
			3: PancakeClickerGlobalController.seller_cost_3 = values[0]
		
		queue_redraw()

func _draw() -> void:
	var max_value = values.max()
	var min_value = values.min()
	var fitted_values = []
	for value_id in range(res):
		fitted_values.append((1 - (values[value_id] - min_value) / (max_value - min_value)) * size.y)
	
	label_min.text = str(round(min_value * 10) / 10)
	label_max.text = str(round(max_value * 10) / 10)
	label_current.text = str(round(values[0] * 10) / 10)
	
	draw_line(Vector2(0, fitted_values[0]), Vector2(size.x, fitted_values[0]), GlobalControlls.global_color_palette.colors[1])
	
	for value_id in range(res - 1):
		draw_line(Vector2(value_id * size.x / (res - 1), fitted_values[value_id]), Vector2((value_id + 1) * size.x / (res - 1), fitted_values[value_id + 1]), GlobalControlls.global_color_palette.colors[0])
