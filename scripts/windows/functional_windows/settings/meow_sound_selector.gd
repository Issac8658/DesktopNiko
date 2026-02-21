extends OptionButton


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	selected = ValuesContainer.CurrentMeowSoundId
	item_selected.connect(func (selected_item):
		ValuesContainer.CurrentMeowSoundId = selected_item
	)
