extends WMPopup

@export var title_label : Label
@export var text_label : Label

func _ready() -> void:
	super()

func show_popup(popup_title : String, text : String) -> void:
	title_label.text = popup_title
	text_label.text = text
	popup()
