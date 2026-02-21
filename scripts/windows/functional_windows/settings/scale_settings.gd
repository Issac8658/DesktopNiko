extends OptionButton

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	selected = ValuesContainer.NikoScale
	item_selected.connect(func (id):
		ValuesContainer.NikoScale = id
	)
