extends Button


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	if not OS.get_name() == "Windows":
		disabled = true
