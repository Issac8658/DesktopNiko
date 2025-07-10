extends Control

@onready var item : ShopItem = get_parent()

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	item.buyed.connect(func ():
		PancakeClickerGlobalController.pancake_layers += 1
		PancakeClickerGlobalController.pancakes_add += 1
		PancakeClickerGlobalController.pancake_update.emit()
		if item.buyed_for < 5:
			item.show_again(int(item.cost * 1.5))
	)
	item.check.connect(func(): 
		if item.buyed_for < 5:
			item.show_again(int(item.cost * pow(1.5, item.buyed_for)))
		else:
			item.set_buyed()
		PancakeClickerGlobalController.pancake_layers = item.buyed_for
	)
