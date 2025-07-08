extends Control

@onready var item : ShopItem = get_parent()

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	item.buyed.connect(func ():
		PancakeClickerGlobalController.yelli_multiplier += 0.1
		if item.buyed_for < 100:
			item.show_again(int(item.cost * 1.1))
	)
	item.check.connect(func(): 
		if item.buyed_for < 100:
			item.show_again(int(item.cost * pow(1.1, item.buyed_for)))
		else:
			visible = false
	)
