extends HBoxContainer

@export var left_button : Button
@export var right_button : Button
@export var grabber_button : Button
@export var fill : Control
@export var fill_zone : Control
@export var slider : Range

var offset = 0

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	await get_tree().process_frame
	grabber_button.gui_input.connect(func (event : InputEvent):
		if event is InputEventMouseMotion:
			if event.button_mask == MOUSE_BUTTON_MASK_LEFT:
				offset += event.relative.x / fill_zone.size.x * (slider.max_value - slider.min_value) * (fill_zone.size.x + 32) / fill_zone.size.x
				slider.value = offset
				update()
		elif event is InputEventMouseButton:
			if not event.is_pressed():
				offset = clamp(offset, slider.min_value, slider.max_value)
	)
	slider.changed.connect(func ():
		update()
	)
	slider.visibility_changed.connect(func ():
		update()
	)
	
	left_button.pressed.connect(func ():
		offset -= slider.step
		offset = clamp(offset, slider.min_value, slider.max_value)
		slider.value = offset
		update()
	)
	right_button.pressed.connect(func ():
		offset += slider.step
		offset = clamp(offset, slider.min_value, slider.max_value)
		slider.value = offset
		update()
	)
	offset = slider.value
	update()


func update():
	await get_tree().process_frame
	await get_tree().process_frame
	var target_size = fill_zone.size.x * (slider.value - slider.min_value) / (slider.max_value - slider.min_value)
	fill.size.x = target_size
	grabber_button.position.x = target_size * (fill_zone.size.x - 32) / fill_zone.size.x
