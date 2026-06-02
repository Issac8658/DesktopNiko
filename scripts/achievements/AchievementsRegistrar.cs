using Godot;
using System.Collections.Generic;

public partial class AchievementsRegistrar : Node
{
	private AchievementsController _achCon;
	private readonly Dictionary<string, Variant[]> Achievements = new(){ // Name, description, icon, is description and name hidden before unlock
		{"ten_clicks", 					["res://sprites/achievements/ten_clicks.png",					false]},
		{"one_hundred_clicks", 			["res://sprites/achievements/one_hundred_clicks.png",			false]},
		{"one_thousand_clicks",			["res://sprites/achievements/one_thousand_clicks.png",			false]},
		{"one_hundred_thousand_clicks", ["res://sprites/achievements/one_hundred_thousand_clicks.png",	false]},
		{"one_million_clicks", 			["res://sprites/achievements/one_million_clicks.png",			false]},
		{"one_billion_clicks", 			["res://sprites/achievements/glitched2.png",					false]},
		{"im_scared", 					["res://sprites/achievements/im_scared.png",					false]},
		{"what_you_do", 				["res://sprites/achievements/what_you_doing.png",				false]},
		{"sweet_dreams", 				["res://sprites/achievements/sweet_dreams.png",					false]},
		{"little_win", 					["res://sprites/achievements/little_win.png",					false]},
		{"master_of_tennis", 			["res://sprites/achievements/master_of_tennis.png",				false]},
		{"hacked_a_machine", 			["res://sprites/achievements/hacked_a_machine.png",				false]},
		{"time_to_dinner", 				["res://sprites/achievements/time_to_dinner.png",				false]},
		{"meow", 						["res://sprites/achievements/meow.png",							false]},
		{"sliding_win", 				["res://sprites/achievements/sliding_puzzle.png",				false]},
		{"everyone_is_dancing", 		["res://sprites/achievements/everyone_is_dancing.png",			false]},
		{"tetris", 						["res://sprites/achievements/tetris.png",						false]},
		{"tetris_oneshot", 				["res://sprites/achievements/oneshot.png",						false]},
		{"tetris_25000", 				["res://sprites/achievements/tetris25000.png",					false]},
		{"few_squares", 				["res://sprites/achievements/glitched2.png",					true]},
		{"worst_friend", 				["res://sprites/achievements/glitched.png",						true]},
		{"hello_secret", 				["res://sprites/achievements/unknown.png",						true]},
		{"cat_window", 					["res://sprites/achievements/cat_window.png",					true]}
	};
	
	public override void _Ready()
	{
		_achCon = GetNode<AchievementsController>("/root/AchievementsController");

		foreach (KeyValuePair<string, Variant[]> AchievementData in Achievements)
		{
			_achCon.RegisterAchievement(new()
			{
				Id = AchievementData.Key,
				Title = "achievements." + AchievementData.Key + ".title",
				Description = "achievements." + AchievementData.Key + ".description",
				Icon = GD.Load<Texture2D>((string)AchievementData.Value[0]),
				Hidden = (bool)AchievementData.Value[1]
			});
		}
	}
}
