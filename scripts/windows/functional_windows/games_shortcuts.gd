extends Node

@export var games : Array[PackedScene]
@export var games_node : Node;

var opened_games : PackedStringArray = []

func _ready():
	var children = get_children()
	for child in children:
		var button = child.get_node("Button")
		if button and games[children.find(child)]: 
			button.gui_input.connect(func (event: InputEvent):
				if event is InputEventMouseButton and event.double_click:
					var packed_game = games[children.find(child)]
					if not opened_games.has(packed_game.resource_path):
						var game = packed_game.instantiate()
						opened_games.append(packed_game.resource_path)
						games_node.add_child(game)
						game.tree_exiting.connect(func():
							opened_games.erase(packed_game.resource_path)
						)
			)
