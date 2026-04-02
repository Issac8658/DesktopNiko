extends Control

const CELLS_COUNT := Vector2i(4, 4)
const ANIM_TIME : float = .25

@export var part_template : PackedScene
@export var parts_gap : int;
@export var restart_button : Button
var parts : Dictionary[Vector2i, Control] = {}

func _ready() -> void:
	for y in range(CELLS_COUNT.y):
		for x in range(CELLS_COUNT.x):
			if Vector2i(x + 1, y + 1) != CELLS_COUNT:
				var pos := Vector2i(x, y)
				var part : Control = part_template.instantiate()
				add_child(part)
				part.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
				update_part(part, pos)
				part.text = str(y * CELLS_COUNT.x + x + 1)
				parts[pos] = part
				
				part.gui_input.connect(func (event):
					if (event is InputEventMouseButton):
						if (event.pressed and event.button_index == MOUSE_BUTTON_LEFT):
							var curr_pos : Vector2i = parts.find_key(part)
							move_part(curr_pos)
							#print(curr_pos)
				)
				part.mouse_entered.connect(func():
					if (DisplayServer.mouse_get_button_state() == MOUSE_BUTTON_MASK_LEFT):
						var curr_pos : Vector2i = parts.find_key(part)
						move_part(curr_pos)
						#print(curr_pos)
				)
	restart_button.pressed.connect(mix)

func move_part(part_pos: Vector2i) -> bool:
	var part = parts.get(part_pos)
	
	if (part != null):
		if (part_pos.y > 0 and not parts.has(part_pos - Vector2i(0, 1))):
				parts.erase(part_pos)
				parts.set(part_pos - Vector2i(0, 1), part)
				smooth_update_part(part, part_pos - Vector2i(0, 1))
		elif (part_pos.y < CELLS_COUNT.y - 1 and not parts.has(part_pos + Vector2i(0, 1))):
				parts.erase(part_pos)
				parts.set(part_pos + Vector2i(0, 1), part)
				smooth_update_part(part, part_pos + Vector2i(0, 1))
		elif (part_pos.x > 0 and not parts.has(part_pos - Vector2i(1, 0))):
				parts.erase(part_pos)
				parts.set(part_pos - Vector2i(1, 0), part)
				smooth_update_part(part, part_pos - Vector2i(1, 0))
		elif (part_pos.x < CELLS_COUNT.x - 1 and not parts.has(part_pos + Vector2i(1, 0))):
				parts.erase(part_pos)
				parts.set(part_pos + Vector2i(1, 0), part)
				smooth_update_part(part, part_pos + Vector2i(1, 0))
		else:
			return false
	else:
		return false
	return true


func update_part(part : Control, pos : Vector2i):
	part.target_pos = Rect2(float(pos.x) / float(CELLS_COUNT.x), float(pos.y) / float(CELLS_COUNT.y), float(pos.x + 1) / float(CELLS_COUNT.x), float(pos.y + 1) / float(CELLS_COUNT.y))
	@warning_ignore("integer_division")
	part.set_anchor_and_offset(SIDE_TOP, float(pos.y) / float(CELLS_COUNT.y), parts_gap / 2)
	@warning_ignore("integer_division")
	part.set_anchor_and_offset(SIDE_BOTTOM, float(pos.y + 1) / float(CELLS_COUNT.y), -parts_gap / 2)
	@warning_ignore("integer_division")
	part.set_anchor_and_offset(SIDE_LEFT, float(pos.x) / float(CELLS_COUNT.x), parts_gap / 2)
	@warning_ignore("integer_division")
	part.set_anchor_and_offset(SIDE_RIGHT, float(pos.x + 1) / float(CELLS_COUNT.x), -parts_gap / 2)

func smooth_update_part(part : Control, pos : Vector2i):
	#part.target_pos = Rect2(float(pos.x) / float(CELLS_COUNT.x), float(pos.y) / float(CELLS_COUNT.y), float(pos.x + 1) / float(CELLS_COUNT.x), float(pos.y + 1) / float(CELLS_COUNT.y))
	#if (old_pos.y != pos.y):
	#	var t1 = part.create_tween()
	#	var t2 = part.create_tween()
	#	t1.set_trans(Tween.TRANS_ELASTIC)
	#	t2.set_trans(Tween.TRANS_ELASTIC)
	#	t1.set_ease(Tween.EASE_OUT)
	#	t2.set_ease(Tween.EASE_OUT)
	#	if (old_pos.y > pos.y):
	#		t1.tween_property(part, "anchor_top", float(pos.y) / float(CELLS_COUNT.y), ANIM_TIME)
	#		t2.tween_property(part, "anchor_bottom", float(pos.y + 1) / float(CELLS_COUNT.y), ANIM_TIME + 0.2)
	#	else:
	#		t1.tween_property(part, "anchor_bottom", float(pos.y + 1) / float(CELLS_COUNT.y), ANIM_TIME)
	#		t2.tween_property(part, "anchor_top", float(pos.y) / float(CELLS_COUNT.y), ANIM_TIME + 0.2)
	#if (old_pos.x != pos.x):
	#	var t1 = part.create_tween()
	#	var t2 = part.create_tween()
	#	t1.set_trans(Tween.TRANS_ELASTIC)
	#	t2.set_trans(Tween.TRANS_ELASTIC)
	#	t1.set_ease(Tween.EASE_OUT)
	#	t2.set_ease(Tween.EASE_OUT)
	#	if (old_pos.x > pos.x):
	#		t1.tween_property(part, "anchor_left", float(pos.x) / float(CELLS_COUNT.x), ANIM_TIME)
	#		t2.tween_property(part, "anchor_right", float(pos.x + 1) / float(CELLS_COUNT.x), ANIM_TIME + 0.2)
	#	elif (old_pos.x != pos.x):
	#		t1.tween_property(part, "anchor_right", float(pos.x + 1) / float(CELLS_COUNT.x), ANIM_TIME)
	#		t2.tween_property(part, "anchor_left", float(pos.x) / float(CELLS_COUNT.x), ANIM_TIME + 0.2)
	var t1 = part.create_tween()
	var t2 = part.create_tween()
	var t3 = part.create_tween()
	var t4 = part.create_tween()
	t1.set_trans(Tween.TRANS_BACK)
	t2.set_trans(Tween.TRANS_BACK)
	t3.set_trans(Tween.TRANS_BACK)
	t4.set_trans(Tween.TRANS_BACK)
	t1.set_ease(Tween.EASE_OUT)
	t2.set_ease(Tween.EASE_OUT)
	t3.set_ease(Tween.EASE_OUT)
	t4.set_ease(Tween.EASE_OUT)
	t1.tween_property(part, "anchor_top", float(pos.y) / float(CELLS_COUNT.y), ANIM_TIME)
	t2.tween_property(part, "anchor_bottom", float(pos.y + 1) / float(CELLS_COUNT.y), ANIM_TIME)
	t4.tween_property(part, "anchor_left", float(pos.x) / float(CELLS_COUNT.x), ANIM_TIME)
	t3.tween_property(part, "anchor_right", float(pos.x + 1) / float(CELLS_COUNT.x), ANIM_TIME)
	
func mix(count : int = 200):
	var i = 0
	
	while (i < count):
		if (move_part(Vector2i(randi_range(0, CELLS_COUNT.x), randi_range(0, CELLS_COUNT.y)))):
			i += 1;
