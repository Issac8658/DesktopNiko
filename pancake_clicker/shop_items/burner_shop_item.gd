extends Control

@onready var item : ShopItem = get_parent()

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	item.buyed.connect(func ():
		item.show_again(int(item.cost * 1.5))
	)
	item.check.connect(func(): 
		item.show_again(int(item.cost * pow(1.5, item.buyed_for - 1)))
	)
