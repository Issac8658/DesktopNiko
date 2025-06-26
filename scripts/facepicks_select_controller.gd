extends Node

func _ready() -> void: # appling saved facepicks
	GlobalControlls.idle_facepick = $Idle/OptionButton.get_item_icon($Idle/OptionButton.selected)
	GlobalControlls.speak_facepick = $Speak/OptionButton.get_item_icon($Speak/OptionButton.selected)
	GlobalControlls.scare_facepick = $Scare/OptionButton.get_item_icon($Scare/OptionButton.selected)
	GlobalControlls.scare_speak_facepick = $ScareSpeak/OptionButton.get_item_icon($ScareSpeak/OptionButton.selected)
	update_facepicks_preview()

func update_facepicks_preview(): # update facepicks preview in settings
	$Idle/TextureRect.texture = $Idle/OptionButton.get_item_icon($Idle/OptionButton.selected)
	$Speak/TextureRect.texture = $Idle/OptionButton.get_item_icon($Speak/OptionButton.selected)
	$Scare/TextureRect.texture = $Idle/OptionButton.get_item_icon($Scare/OptionButton.selected)
	$ScareSpeak/TextureRect.texture = $Idle/OptionButton.get_item_icon($ScareSpeak/OptionButton.selected)

func _on_idle_item_selected(index: int) -> void: # on changing idle facepick
	GlobalControlls.idle_facepick = $Idle/OptionButton.get_item_icon(index)
	update_facepicks_preview()
	GlobalControlls.facepick_update.emit()

func _on_speak_item_selected(index: int) -> void: # on changing speak facepick
	GlobalControlls.speak_facepick = $Speak/OptionButton.get_item_icon(index)
	update_facepicks_preview()

func _on_scare_item_selected(index: int) -> void: # on changing scared facepick
	GlobalControlls.scare_facepick = $Scare/OptionButton.get_item_icon(index)
	update_facepicks_preview()

func _on_option_button_item_selected(index: int) -> void: # on changing scared speak facepick
	GlobalControlls.scare_speak_facepick = $ScareSpeak/OptionButton.get_item_icon(index)
	update_facepicks_preview()
