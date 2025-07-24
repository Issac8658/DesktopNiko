extends Node

var niko_sprites = {
	"niko" = load("res://niko/niko.png"),
	"niko_cry" = load("res://niko/niko_cry.png"),
	"niko_default" = load("res://niko/niko_default.png"),
	"niko_gasmask" = load("res://niko/niko_gasmask.png"),
	"niko_look_left" = load("res://niko/niko_look_left.png"),
	"niko_look_right" = load("res://niko/niko_look_right.png"),
	"niko_pancakes" = load("res://niko/niko_pancakes.png"),
	"niko_sad" = load("res://niko/niko_sad.png"),
	"niko_shock" = load("res://niko/niko_shock.png"),
	"niko_sleepy" = load("res://niko/niko_sleepy.png"),
	"niko_sleep" = load("res://niko/niko_sleep.png"),
	"niko_smile" = load("res://niko/niko_smile.png"),
	"niko_smug" = load("res://niko/niko_smug.png"),
	"niko_speak" = load("res://niko/niko_speak.png"),
	"niko_surprised" = load("res://niko/niko_surprised.png")
}

var niko_legacy_sprites = {
	"niko" = load("res://niko/niko_legacy/niko.png"),
	"niko_cry" = load("res://niko/niko_legacy/niko_cry.png"),
	"niko_default" = load("res://niko/niko_legacy/niko_default.png"),
	"niko_gasmask" = load("res://niko/niko_legacy/niko_gasmask.png"),
	"niko_look_left" = load("res://niko/niko_legacy/niko_look_left.png"),
	"niko_look_right" = load("res://niko/niko_legacy/niko_look_right.png"),
	"niko_pancakes" = load("res://niko/niko_legacy/niko_pancakes.png"),
	"niko_sad" = load("res://niko/niko_legacy/niko_sad.png"),
	"niko_shock" = load("res://niko/niko_legacy/niko_shock.png"),
	"niko_sleepy" = load("res://niko/niko_legacy/niko_sleepy.png"),
	"niko_sleep" = load("res://niko/niko_legacy/niko_sleep.png"),
	"niko_smile" = load("res://niko/niko_legacy/niko_smile.png"),
	"niko_smug" = load("res://niko/niko_legacy/niko_smug.png"),
	"niko_speak" = load("res://niko/niko_legacy/niko_speak.png"),
	"niko_surprised" = load("res://niko/niko_legacy/niko_surprised.png")
}

func get_sprite(sprite_id : String) -> Resource:
	if GlobalControlls.use_legacy_sprites:
		if sprite_id in niko_legacy_sprites.keys():
			return niko_legacy_sprites[sprite_id]
		else:
			print("sprite " + sprite_id + " is does not exist (legacy)")
			return niko_legacy_sprites["niko"]
	else:
		if sprite_id in niko_sprites.keys():
			return niko_sprites[sprite_id]
		else:
			print("sprite " + sprite_id + " is does not exist")
			return niko_sprites["niko"]


func has_sprite(sprite_id : String) -> bool:
	if GlobalControlls.use_legacy_sprites:
		return sprite_id in niko_legacy_sprites.keys()
	else:
		return sprite_id in niko_sprites.keys()
