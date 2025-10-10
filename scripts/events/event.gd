@icon("res://sprites/icons/error.png")
extends Node
class_name Event

signal event_ended()

@export var event_controller : Node

func event():
	if event_controller:
		if event_controller.has_method("do_event"): # For GDScript
			event_controller.do_event()
		elif event_controller.has_method("DoEvent"): # For C#
			event_controller.DoEvent()
	if event_controller.has_signal("called_to_clear"):
		event_controller.called_to_clear.connect(func():
			event_ended.emit()
		)
