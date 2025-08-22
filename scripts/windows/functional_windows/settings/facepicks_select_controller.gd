extends Node

const facepicks = {
	"niko_smile" = "SMILE",
	"niko_speak" = "SPEAK",
	"niko_shock" = "SHOCK",
	"niko_surprised" = "SURPRISED",
	"niko_gasmask" = "GASMASK",
	"niko_pancakes" = "PANCAKES",
	"niko_default" = "SIMPLE",
	"niko" = "NIKO",
	"niko_smug" = "SMUG"
}

@export var OptionButtons : Array[OptionButton] = []

func _ready() -> void: # appling saved facepicks
	for option_button in OptionButtons:
		for facepick in facepicks:
			option_button.add_icon_item(NikoSpritesModule.get_sprite(facepick), facepicks[facepick])
	
	OptionButtons[0].selected = facepicks.keys().find(GlobalControlls.idle_facepick)
	OptionButtons[1].selected = facepicks.keys().find(GlobalControlls.speak_facepick)
	OptionButtons[2].selected = facepicks.keys().find(GlobalControlls.scare_facepick)
	OptionButtons[3].selected = facepicks.keys().find(GlobalControlls.scare_speak_facepick)
	update_facepicks_preview()

func update_facepicks_preview(): # update facepicks preview in settings
	$Idle/TextureRect.texture = NikoSpritesModule.get_sprite(GlobalControlls.idle_facepick)
	$Speak/TextureRect.texture = NikoSpritesModule.get_sprite(GlobalControlls.speak_facepick)
	$Scare/TextureRect.texture = NikoSpritesModule.get_sprite(GlobalControlls.scare_facepick)
	$ScareSpeak/TextureRect.texture = NikoSpritesModule.get_sprite(GlobalControlls.scare_speak_facepick)

func _on_idle_item_selected(index: int) -> void: # on changing idle facepick
	GlobalControlls.idle_facepick = facepicks.keys()[index]
	update_facepicks_preview()
	GlobalControlls.facepick_update.emit()

func _on_speak_item_selected(index: int) -> void: # on changing speak facepick
	GlobalControlls.speak_facepick = facepicks.keys()[index]
	update_facepicks_preview()

func _on_scare_item_selected(index: int) -> void: # on changing scared facepick
	GlobalControlls.scare_facepick = facepicks.keys()[index]
	update_facepicks_preview()

func _on_scare_speak_item_selected(index: int) -> void: # on changing scared speak facepick
	GlobalControlls.scare_speak_facepick = facepicks.keys()[index]
	update_facepicks_preview()
