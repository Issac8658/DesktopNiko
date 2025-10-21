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
	
	OptionButtons[0].selected = facepicks.keys().find(ValuesContainer.IdleFacepick)
	OptionButtons[1].selected = facepicks.keys().find(ValuesContainer.SpeakFacepick)
	OptionButtons[2].selected = facepicks.keys().find(ValuesContainer.ScareFacepick)
	OptionButtons[3].selected = facepicks.keys().find(ValuesContainer.ScareSpeakFacepick)
	update_facepicks_preview()

func update_facepicks_preview(): # update facepicks preview in settings
	$Idle/TextureRect.texture = NikoSpritesModule.get_sprite(ValuesContainer.IdleFacepick)
	$Speak/TextureRect.texture = NikoSpritesModule.get_sprite(ValuesContainer.SpeakFacepick)
	$Scare/TextureRect.texture = NikoSpritesModule.get_sprite(ValuesContainer.ScareFacepick)
	$ScareSpeak/TextureRect.texture = NikoSpritesModule.get_sprite(ValuesContainer.ScareSpeakFacepick)

func _on_idle_item_selected(index: int) -> void: # on changing idle facepick
	ValuesContainer.IdleFacepick = facepicks.keys()[index]
	update_facepicks_preview()

func _on_speak_item_selected(index: int) -> void: # on changing speak facepick
	ValuesContainer.SpeakFacepick = facepicks.keys()[index]
	update_facepicks_preview()

func _on_scare_item_selected(index: int) -> void: # on changing scared facepick
	ValuesContainer.ScareFacepick = facepicks.keys()[index]
	update_facepicks_preview()

func _on_scare_speak_item_selected(index: int) -> void: # on changing scared speak facepick
	ValuesContainer.ScareSpeakFacepick = facepicks.keys()[index]
	update_facepicks_preview()
