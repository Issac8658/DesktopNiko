@tool
extends Control

@onready var label : Label = $Label
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	custom_minimum_size = label.size
	label.resized.connect(func ():
		custom_minimum_size = label.size
	)
