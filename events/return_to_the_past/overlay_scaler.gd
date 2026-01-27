extends Window

var parent_window : Window

@export var offsets : Rect2i

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	parent_window = get_parent();
	
	parent_window.size_changed.connect(update)
	
	show()
	update()

func update():
	position = parent_window.position - offsets.position
	size = parent_window.size + offsets.position + offsets.size
