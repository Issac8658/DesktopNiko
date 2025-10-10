extends Node

@export var games : Array[PackedScene]
@export var games_node : Node;

func _ready():
	var children = get_children()
	for child in children:
		var button = child.get_node("Button")
		if button and games[children.find(child)]: 
			button.gui_input.connect(func (event: InputEvent):
				if event is InputEventMouseButton and event.double_click:
					var game = games[children.find(child)].instantiate()
					games_node.add_child(game)
			)
