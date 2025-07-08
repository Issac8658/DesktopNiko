extends Panel

const left_panel_poses = [0, 292, 0, 292]
const right_panel_poses = [404, 696, 708, 1000]

@export var window : Window
@export var pancake_animator : AnimationPlayer
@export var pancake_add_template_label : PackedScene
@export_group("Top Bar")
@export var pancakes_count_label : Label
@export var yellis_count_label : Label
@export_group("Panels")
@export_subgroup("Left Panel", "left")
@export var left_panel : Control
@export var left_panel_grabber : Control
@export_subgroup("Right Panel", "right")
@export var right_panel : Control
@export var right_panel_grabber : Control

var left_pos = 0
var right_pos = 0
var left_panel_dragging = false
var left_mouse_offset = 0
var right_panel_dragging = false
var right_mouse_offset = 0

var labels_list = []

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pancake_animator.animation_finished.connect(func (_anim):
		var capcakes = roundi((1 + PancakeClickerGlobalController.pancakes_add) * PancakeClickerGlobalController.pancakes_multiplier)
		PancakeClickerGlobalController.pancakes += capcakes
		update_labels()
		show_pancake_add_label(capcakes)
	)
	# Panels
	left_panel_grabber.gui_input.connect(func (event):
		if event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_LEFT:
			if event.is_pressed():
				left_panel_dragging = true
				left_mouse_offset = event.position.x
			else:
				left_panel_dragging = false
		elif event is InputEventMouseMotion and left_panel_dragging:
			left_pos += left_mouse_offset - event.position.x
			left_pos = clamp(left_pos, 0, left_panel_poses[1] - left_panel_poses[0])
			update_left_panel()
	)
	
	right_panel_grabber.gui_input.connect(func (event):
		if event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_LEFT:
			if event.is_pressed():
				right_panel_dragging = true
				right_mouse_offset = event.position.x
			else:
				right_panel_dragging = false
		if event is InputEventMouseMotion and right_panel_dragging:
			right_pos += event.position.x - right_mouse_offset
			right_pos = clamp(right_pos, 0, right_panel_poses[1] - right_panel_poses[0])
			update_right_panel()
	)
	update_left_panel()
	update_right_panel()
	
	PancakeClickerGlobalController.labels_update.connect(update_labels)
	update_labels()


func update_labels():
	pancakes_count_label.text = str(PancakeClickerGlobalController.pancakes)
	yellis_count_label.text = str(PancakeClickerGlobalController.format_big_number(round(PancakeClickerGlobalController.yelli * 10) / 10))


func _process(_delta: float) -> void:
	for label in labels_list:
		label.position += Vector2.UP


func _on_pancake_pressed() -> void:
	pancake_animator.play("pancake_click")


func update_left_panel():
	var pos = left_panel_poses[1] - left_pos
	left_panel.position.x = pos
	window.mouse_passthrough_polygon[9].x = pos
	window.mouse_passthrough_polygon[10].x = pos

func update_right_panel():
	right_panel.position.x = right_panel_poses[0] + right_pos
	window.mouse_passthrough_polygon[3].x = right_panel_poses[2] + right_pos
	window.mouse_passthrough_polygon[4].x = right_panel_poses[2] + right_pos


func show_pancake_add_label(value : int):
	var label = pancake_add_template_label.instantiate()
	add_child(label)
	label.position = Vector2(randf_range(100, size.x - label.size.x - 100), randf_range(100, size.y - label.size.y - 100))
	label.get_node("Label").text = "+" + str(value)
	labels_list.append(label)
	var label_animator : AnimationPlayer = label.get_node("AnimationPlayer")
	label_animator.play("fade_out")
	label_animator.animation_finished.connect(func (_anim):
		label.queue_free()
		labels_list.erase(label)
	)
