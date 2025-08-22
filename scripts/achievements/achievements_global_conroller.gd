extends Node

signal achievement_taked(achievement_id : String)

const achievements : Dictionary = {
	"ten_clicks" = ["ACHIEVEMENT_1_NAME","ACHIEVEMENT_1_DESC","res://ui_sprites/achievements/ten_clicks.png", false], # Name, description, icon, is description hidden before unlock
	"one_hundred_clicks" = ["ACHIEVEMENT_2_NAME","ACHIEVEMENT_2_DESC","res://ui_sprites/achievements/one_hundred_clicks.png", false],
	"one_thousand_clicks" = ["ACHIEVEMENT_3_NAME","ACHIEVEMENT_3_DESC","res://ui_sprites/achievements/one_thousand_clicks.png", false],
	"one_hundred_thousand_clicks" = ["ACHIEVEMENT_4_NAME","ACHIEVEMENT_4_DESC","res://ui_sprites/achievements/one_hundred_thousand_clicks.png", false],
	"one_million_clicks" = ["ACHIEVEMENT_5_NAME","ACHIEVEMENT_5_DESC","res://ui_sprites/achievements/one_million_clicks.png", false],
	"one_billion_clicks" = ["ACHIEVEMENT_6_NAME","ACHIEVEMENT_6_DESC","res://ui_sprites/achievements/glitched.png", false],
	"im_scared" = ["ACHIEVEMENT_IM_SCARED_NAME","ACHIEVEMENT_IM_SCARED_DESC","res://ui_sprites/achievements/im_scared.png", false],
	"what_you_do" = ["ACHIEVEMENT_WHAT_YOU_DO_NAME","ACHIEVEMENT_WHAT_YOU_DO_DESC","res://ui_sprites/achievements/what_you_doing.png", false],
	"sweet_dreams" = ["ACHIEVEMENT_SWEET_DREAMS_NAME","ACHIEVEMENT_SWEET_DREAMS_DESC","res://ui_sprites/achievements/sweet_dreams.png", false],
	"little_win" = ["ACHIEVEMENT_LITTLE_WIN_NAME","ACHIEVEMENT_LITTLE_WIN_DESC", "res://ui_sprites/achievements/little_win.png", false],
	"master_of_tennis" = ["ACHIEVEMENT_MASTER_OF_TENNIS_NAME","ACHIEVEMENT_MASTER_OF_TENNIS_DESC", "res://ui_sprites/achievements/master_of_tennis.png", false],
	"hacked_a_machine" = ["ACHIEVEMENT_HACKED_A_MACHINE_NAME","ACHIEVEMENT_HACKED_A_MACHINE_DESC", "res://ui_sprites/achievements/hacked_a_machine.png", false],
	"time_to_dinner" = ["ACHIEVEMENT_TIME_TO_DINNER_NAME","ACHIEVEMENT_TIME_TO_DINNER_DESC", "res://ui_sprites/achievements/time_to_dinner.png", false],
	"worst_friend" = ["ACHIEVEMENT_WORST_FRIEND_NAME","ACHIEVEMENT_WORST_FRIEND_DESC", "res://ui_sprites/achievements/glitched.png", true],
	"hello_secret" = ["ACHIEVEMENT_HELLO_NAME","ACHIEVEMENT_HELLO_DESC","res://ui_sprites/achievements/unknown.png", true]
}

var taked_achievements : Array = []


func take_achievement(achievement_id : String) -> bool: # returns has or not
	if achievement_id in taked_achievements:
		print("Achivement \"" + achievement_id + "\" has already been obtained before! (take_achievement)")
		return false
	else:
		if achievement_id in achievements.keys():
			taked_achievements.append(achievement_id)
			achievement_taked.emit(achievement_id)
			return true
		else:
			print("Achievement \"" + achievement_id + "\" does not exist! (take_achievement)")
			return false


func is_achievement_taked(achievement_id : String) -> bool:
	if achievement_id in achievements.keys():
		if achievement_id in taked_achievements:
			return true
		else:
			return false
	else:
		print("Achievement \"" + achievement_id + "\" does not exist! (is_achievement_taked)")
		return false


func get_achievement_data(achievement_id : String) -> Array:
	if achievement_id in achievements.keys():
		return achievements[achievement_id]
	else:
		print("Achievement \"" + achievement_id + "\" does not exist! (get_achievement_data)")
		return ["", "", ""]
