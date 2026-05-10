extends Button

@export var skins_window : Window

func _ready() -> void:
	pressed.connect(func ():
		skins_window.show()
	)
