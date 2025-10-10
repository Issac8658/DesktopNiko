extends Node

const LONG_NUMBER_NAMES = ["K","M","B","T","Qd","Qn","Sx","Sp","O","N","De","Ud","Dd"]
const SAVE_FILE_PATH = "user://PancakeClicker.cfg"
var pancakes_save_file = ConfigFile.new()

var shop_items = {}

@warning_ignore("unused_signal")
signal labels_update()
@warning_ignore("unused_signal")
signal item_buyed(item_id)
@warning_ignore("unused_signal")
signal pancake_update()
signal update_items_buyed_count()
signal info_panel_show_request(info_panel : PackedScene)

var pancakes : int = 0
var yelli : float = 0
var selected_syrup : int = 0
var pancake_layers : int = 0
var pancake_type : int = 0
var has_syrup = false
var has_butter = false

var seller_cost_1 : float = 0
var seller_cost_2 : float = 0
var seller_cost_3 : float = 0

var pancakes_multiplier : float = 1
var pancakes_add : int = 0
var yelli_multiplier : float = 1
var yelli_add : float = 0

func _ready() -> void:
	GlobalControlls.scripts_to_save.append(PancakeClickerGlobalController)
	
	pancakes_save_file.load(SAVE_FILE_PATH)
	pancakes = pancakes_save_file.get_value("Values", "Pancakes", 0)
	yelli = pancakes_save_file.get_value("Values", "Yelli", 0)
	pancakes_multiplier = pancakes_save_file.get_value("Values", "PancakesMultiplier", 1)
	pancakes_add = pancakes_save_file.get_value("Values", "PancakesAdd", 0)
	yelli_multiplier = pancakes_save_file.get_value("Values", "YelliMultiplier", 1)
	yelli_add = pancakes_save_file.get_value("Values", "YelliAdd", 0)
	selected_syrup = pancakes_save_file.get_value("Values", "Syrup", 0)
	
	if pancakes_save_file.has_section("ShopItems"):
		for shop_item_id in pancakes_save_file.get_section_keys("ShopItems"):
			var value = pancakes_save_file.get_value("ShopItems", shop_item_id)
			shop_items[shop_item_id] = value
	update_items_buyed_count.emit()
	
	labels_update.emit()


func save():
	pancakes_save_file.set_value("Values", "Pancakes", pancakes)
	pancakes_save_file.set_value("Values", "Yelli", yelli)
	pancakes_save_file.set_value("Values", "PancakesMultiplier", pancakes_multiplier)
	pancakes_save_file.set_value("Values", "PancakesAdd", pancakes_add)
	pancakes_save_file.set_value("Values", "YelliMultiplier", yelli_multiplier)
	pancakes_save_file.set_value("Values", "YelliAdd", yelli_add)
	pancakes_save_file.set_value("Values", "Syrup", selected_syrup)
	var shop_items_names = shop_items.keys()
	for i in range(len(shop_items)):
		pancakes_save_file.set_value("ShopItems", shop_items_names[i], shop_items[shop_items_names[i]])
	pancakes_save_file.save(SAVE_FILE_PATH)


func format_big_number(number) -> String:
	if number >= 1000:
		number = roundi(number)
		var degree = floor(log10(number) + 0.01)
		var target_name_id = floori(degree / 3.0)
		var multiplier = number / pow(10, target_name_id * 3)
		var floored_multiplier
		if multiplier - floor(multiplier) < 0.1:
			floored_multiplier = floori(multiplier)
		else:
			floored_multiplier = floor(multiplier * 10) / 10
		var formated_number = str(floored_multiplier) + LONG_NUMBER_NAMES[target_name_id - 1]
		if multiplier - floored_multiplier > 0:
			formated_number += "+"
		return formated_number
	else:
		return str(number)


func show_info_panel(info : PackedScene):
	info_panel_show_request.emit(info)


func log10(number : float) -> float:
	return log(number) / log(10)


func bool_collection_to_number(bools : Array) -> int:
	if len(bools) < 8:
		for i in range(8 - len(bools)):
			bools.append(false)
	var number = 0
	for i in range(8):
		number += int(bools[i]) * pow(2, 7 - i)
	return number
