@icon("res://ui_sprites/doc.png")
extends PanelContainer
class_name ShopItem

signal buyed()
@warning_ignore("unused_signal")
signal check()

@export var cost : int = 0
@export var use_pancakes : bool = false
@export var buy_button : Button
@export var cost_label : Label
@export_group("Data")
@export var item_id : String = "replace_this_string_to_unique"
@export var buyed_for : int = 0
@export var once_buy : bool = true

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	
	update_cost()
	buy_button.pressed.connect(func ():
		if not use_pancakes:
			if PancakeClickerGlobalController.yelli >= cost:
				PancakeClickerGlobalController.yelli -= cost
				buy()
		else:
			if PancakeClickerGlobalController.pancakes >= cost:
				PancakeClickerGlobalController.pancakes -= cost
				buy()
	)


func buy():
	visible = false
	buyed_for += 1
	if once_buy:
		PancakeClickerGlobalController.shop_items[item_id] = true
	else:
		PancakeClickerGlobalController.shop_items[item_id] = buyed_for
	buyed.emit()
	PancakeClickerGlobalController.labels_update.emit()


func show_again(new_cost : int):
	cost = new_cost
	update_cost()
	visible = true


func update_cost():
	cost_label.text = PancakeClickerGlobalController.format_big_number(cost)
