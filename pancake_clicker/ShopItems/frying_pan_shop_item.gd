extends Control

@onready var item : ShopItem = get_parent()

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	item.buyed.connect(func ():
		PancakeClickerGlobalController.pancakes_add += 1
	)
