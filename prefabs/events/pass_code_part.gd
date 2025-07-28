extends VBoxContainer

@export var symbols_list : Array[String]
@export var up_button : Button
@export var label : Label
@export var down_button : Button
@export var current_symbol : int = 0;

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	up_button.pressed.connect(func ():
		current_symbol += 1
		update()
	)
	down_button.pressed.connect(func ():
		current_symbol -= 1
		update()
	)

func update():
	if current_symbol >= len(symbols_list):
		current_symbol = 0
	if current_symbol < 0:
		current_symbol = len(symbols_list) - 1
	label.text = symbols_list[current_symbol]
