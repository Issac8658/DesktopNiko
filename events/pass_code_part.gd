extends Node

signal value_changed()

var symbol = 0

@export var symbols_list : Array[String]
@export var up_button : Button
@export var label : Label
@export var down_button : Button
@export var current_symbol : int:
	get:
		return symbol
	set(value):
		symbol = value
		update()

func _ready() -> void:
	up_button.pressed.connect(func ():
		symbol += 1
		update()
	)
	down_button.pressed.connect(func ():
		symbol -= 1
		update()
	)

func update():
	if symbol >= len(symbols_list):
		symbol = 0
	if current_symbol < 0:
		symbol = len(symbols_list) - 1
	label.text = symbols_list[symbol]
	
	value_changed.emit()
