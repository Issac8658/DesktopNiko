extends TabBar

@export var pages : Array[CanvasItem]

func _ready() -> void:
	for i in range(len(pages)):
		pages[i].visible = current_tab == i
	tab_selected.connect(func (_tab_id):
		for i in range(len(pages)):
			pages[i].visible = current_tab == i
	)
