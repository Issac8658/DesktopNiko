extends Control


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	var children = $ItemsContainer.get_children()
	for seller_id in range(len(children)):
		var child = children[seller_id]
		var buttons_container = child.get_node("VBoxContainer/HBoxContainer")
		buttons_container.get_node("SellButton").pressed.connect(func ():
			if PancakeClickerGlobalController.pancakes > 0:
				PancakeClickerGlobalController.pancakes -= 1
				match seller_id:
					0: PancakeClickerGlobalController.yelli += PancakeClickerGlobalController.seller_cost_1
					1: PancakeClickerGlobalController.yelli += PancakeClickerGlobalController.seller_cost_2
					2: PancakeClickerGlobalController.yelli += PancakeClickerGlobalController.seller_cost_3
				
				PancakeClickerGlobalController.labels_update.emit()
		)
		
		buttons_container.get_node("SellAllButton").pressed.connect(func ():
			match seller_id:
				0: PancakeClickerGlobalController.yelli += PancakeClickerGlobalController.seller_cost_1 * PancakeClickerGlobalController.pancakes
				1: PancakeClickerGlobalController.yelli += PancakeClickerGlobalController.seller_cost_2 * PancakeClickerGlobalController.pancakes
				2: PancakeClickerGlobalController.yelli += PancakeClickerGlobalController.seller_cost_3 * PancakeClickerGlobalController.pancakes
			PancakeClickerGlobalController.pancakes = 0
			
			PancakeClickerGlobalController.labels_update.emit()
		)
