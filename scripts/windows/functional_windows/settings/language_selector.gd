extends OptionButton

func _ready() -> void:
	select(ValuesContainer.Language)
	item_selected.connect(func (ix):
		ValuesContainer.Language = ix
	)
