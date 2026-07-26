extends Node

@export var pages_container : Node;
@export var forward_button : Button;
@export var backward_button : Button;
@export var counter_label : Label;

var page : int = 0;

func _ready() -> void:
	_update_pages();
	if counter_label != null:
		counter_label.text = str(page);

	backward_button.pressed.connect(func ():
		page = (page - 1) % pages_container.get_child_count();
		if page < 0:
			page = pages_container.get_child_count() + page
		_update_pages();
		if counter_label != null:
			counter_label.text = str(page + 1);
	)
	forward_button.pressed.connect(func ():
		page = (page + 1) % pages_container.get_child_count();
		_update_pages();
		if counter_label != null:
			counter_label.text = str(page + 1);
	)
	
	pages_container.child_entered_tree.connect(func (_node): _update_pages());
	pages_container.child_exiting_tree.connect(func (_node): _update_pages());

func _update_pages() -> void:
	var pages_count : int = pages_container.get_child_count()
	
	for i in range(pages_count):
		pages_container.get_child(i).visible = false;
	pages_container.get_child(page).visible = true;
	
	if pages_count <= 1:
		backward_button.visible = false;
		forward_button.visible = false;
	else:
		backward_button.visible = true;
		forward_button.visible = true;
