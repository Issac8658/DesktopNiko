extends Button

@export var on_icon : Texture2D
@export var off_icon : Texture2D
var parent : Button

var hovered := false

func _ready() -> void:
	parent = get_parent()
	parent.toggled.connect(update_icon)
	update_icon(parent.button_pressed)
	
	parent.mouse_entered.connect(func ():
		theme_type_variation = "AlwaysActivatedButton"
		hovered = true
	)
	parent.mouse_exited.connect(func ():
		theme_type_variation = "IconButton"
		hovered = false
	)
	parent.button_down.connect(func ():
		theme_type_variation = "IconButton"
	)
	parent.button_up.connect(func ():
		if (hovered):
			theme_type_variation = "AlwaysActivatedButton"
	)

func update_icon(state: bool):
	if state:
		icon = on_icon
	else:
		icon = off_icon
