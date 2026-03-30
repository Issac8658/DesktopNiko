extends Control

const CELLS_COUNT := Vector2i(4, 4)

@export var part_template : PackedScene
@export var parts_gap : int;
var parts : Dictionary[Vector2i, Control] = {}

func _ready() -> void:
	for y in range(CELLS_COUNT.y):
		for x in range(CELLS_COUNT.x):
			if Vector2i(x + 1, y + 1) != CELLS_COUNT:
				var part : Control = part_template.instantiate()
				add_child(part)
				part.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
				update_part(part, Vector2i(x, y))
				part.text = str(y * CELLS_COUNT.x + x + 1)
				parts[Vector2i(x,y)] = part

func move_part(part_pos: Vector2i):
	if (not parts.has(part_pos - Vector2i(0, 1))):
			pass


func update_part(part : Control, pos : Vector2i):
	@warning_ignore("integer_division")
	part.set_anchor_and_offset(SIDE_TOP, float(pos.y) / float(CELLS_COUNT.y), parts_gap / 2)
	@warning_ignore("integer_division")
	part.set_anchor_and_offset(SIDE_BOTTOM, float(pos.y + 1) / float(CELLS_COUNT.y), -parts_gap / 2)
	@warning_ignore("integer_division")
	part.set_anchor_and_offset(SIDE_LEFT, float(pos.x) / float(CELLS_COUNT.x), parts_gap / 2)
	@warning_ignore("integer_division")
	part.set_anchor_and_offset(SIDE_RIGHT, float(pos.x + 1) / float(CELLS_COUNT.x), -parts_gap / 2)
