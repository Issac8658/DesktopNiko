extends Control


func _ready() -> void:
	PancakeClickerGlobalController.item_buyed.connect(update)
	update("what you doing here?")

func update(_item_id):
	if PancakeClickerGlobalController.shop_items.has("butter"):
		visible = PancakeClickerGlobalController.shop_items["butter"]
		PancakeClickerGlobalController.has_butter = PancakeClickerGlobalController.shop_items["butter"]
		PancakeClickerGlobalController.pancake_update.emit()
	else:
		visible = false
		PancakeClickerGlobalController.has_butter = false
		PancakeClickerGlobalController.pancake_update.emit()
