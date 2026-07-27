extends CheckBox

func _ready() -> void:
	button_pressed = ValuesContainer.PanelIsFlipped;
	toggled.connect(func (toggled_on):
		ValuesContainer.PanelIsFlipped = toggled_on;
	)
