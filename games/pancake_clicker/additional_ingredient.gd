extends Button

enum pancake_types{default, hazelnut, chocolate}

@export var pancake_type : pancake_types = pancake_types.default

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pressed.connect(func ():
		PancakeClickerGlobalController.pancake_type = pancake_type
		PancakeClickerGlobalController.pancake_update.emit()
	)
	PancakeClickerGlobalController.item_buyed.connect(func (_item_id):
		update()
	)
	PancakeClickerGlobalController.pancake_update.connect(func ():
		if PancakeClickerGlobalController.pancake_type == int(pancake_type):
			disabled = true
		else:
			disabled = false
	)
	update()


func update():
	if not pancake_type == pancake_types.default:
		if PancakeClickerGlobalController.shop_items.has(pancake_types.keys()[pancake_type]):
			visible = true
		else:
			visible = false
