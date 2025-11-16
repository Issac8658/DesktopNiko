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
#	for option_button in OptionButtons:
#		for facepick in facepicks:
#			option_button.add_icon_item(NikoSpritesModule.get_sprite(facepick), facepicks[facepick])
	
	OptionButtons[0].selected = facepicks.keys().find(ValuesContainer.IdleFacepic)
	OptionButtons[1].selected = facepicks.keys().find(ValuesContainer.SpeakFacepic)
	OptionButtons[2].selected = facepicks.keys().find(ValuesContainer.ScareFacepic)
	OptionButtons[3].selected = facepicks.keys().find(ValuesContainer.ScareSpeakFacepic)
	update_facepicks_preview()

func update_facepicks_preview(): # update facepicks preview in settings
	$Idle/TextureRect.texture = NikoSkinManager.GetCurrentSkinSprite(ValuesContainer.IdleFacepic)
	$Speak/TextureRect.texture = NikoSkinManager.GetCurrentSkinSprite(ValuesContainer.SpeakFacepic)
	$Scare/TextureRect.texture = NikoSkinManager.GetCurrentSkinSprite(ValuesContainer.ScareFacepic)
	$ScareSpeak/TextureRect.texture = NikoSkinManager.GetCurrentSkinSprite(ValuesContainer.ScareSpeakFacepic)

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
