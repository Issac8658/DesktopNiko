extends Button

enum syrup_types{maple, strawberry, chocolate, caramel, apple, blueberry}

@export var syrup_type : syrup_types = syrup_types.maple

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pressed.connect(func ():
		PancakeClickerGlobalController.selected_syrup = syrup_type
		PancakeClickerGlobalController.pancake_update.emit()
	)
	PancakeClickerGlobalController.item_buyed.connect(func (_item_id):
		update()
	)
	PancakeClickerGlobalController.pancake_update.connect(func ():
		if PancakeClickerGlobalController.selected_syrup == int(syrup_type):
			disabled = true
		else:
			disabled = false
	)
	update()


func update():
	if PancakeClickerGlobalController.shop_items.has("syrup_" + syrup_types.keys()[syrup_type]):
		visible = true
		PancakeClickerGlobalController.has_syrup = true
	else:
		visible = false
