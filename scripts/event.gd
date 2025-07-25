@icon("res://ui_sprites/error.png")
extends Node
class_name Event

@export var event_controller : Node

@export var spawn_chance : float = 0.01
@export var min_clicks : int = 100
@export var max_cps : int = 100

func event():
	if event_controller:
		if event_controller.has_method("do_event"):
			event_controller.do_event()
