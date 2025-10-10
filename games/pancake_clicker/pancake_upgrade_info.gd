extends Control

var current_panel : Node

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	PancakeClickerGlobalController.info_panel_show_request.connect(func (info_panel : PackedScene):
		if current_panel:
			current_panel.queue_free()
		current_panel = info_panel.instantiate()
		add_child(current_panel)
	)
