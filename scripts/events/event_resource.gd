extends Resource
class_name PackedEvent

@export var spawn_chance : float = 0.01
@export var min_clicks : int = 100
@export var max_cps : int = 100

@export var display_name : String = "Unnamed Event" # for events library

@export var linked_scene : PackedScene
